using GameOfLife.Domain.Common;
using GameOfLife.Domain.Domain;
using GameOfLife.Application;
using GameOfLife.Application.CreateUniverse;
using GameOfLife.Application.GetFinalState;
using GameOfLife.Application.GetGeneration;
using GameOfLife.Application.GetUniverse;
using GameOfLife.Api.Concurrency;
using GameOfLife.Api.Contracts;
using GameOfLife.Api.Mapping;
using GameOfLife.Api.Options;
using GameOfLife.Api.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace GameOfLife.Api.Controllers;

/// <summary>
/// Translates HTTP onto the four use cases. Contains no business logic: it binds, validates the
/// boundary caps, calls a handler, and maps the Result onto a status code.
/// </summary>
[ApiController]
[Route("api/v1/boards")]
public sealed class BoardsController : ControllerBase
{
    private readonly ICommandHandler<CreateUniverseCommand> _createHandler;
    private readonly IQueryHandler<GetUniverseQuery, UniverseView> _getUniverseHandler;
    private readonly IQueryHandler<GetGenerationQuery, PatternView> _getGenerationHandler;
    private readonly IQueryHandler<GetFinalStateQuery, FinalStateView> _getFinalStateHandler;
    private readonly IEvaluationAdmissionGate _admissionGate;
    private readonly LinkGenerator _linkGenerator;
    private readonly IOptions<GameOfLifeOptions> _options;

    public BoardsController(
        ICommandHandler<CreateUniverseCommand> createHandler,
        IQueryHandler<GetUniverseQuery, UniverseView> getUniverseHandler,
        IQueryHandler<GetGenerationQuery, PatternView> getGenerationHandler,
        IQueryHandler<GetFinalStateQuery, FinalStateView> getFinalStateHandler,
        IEvaluationAdmissionGate admissionGate,
        LinkGenerator linkGenerator,
        IOptions<GameOfLifeOptions> options)
    {
        _createHandler = createHandler;
        _getUniverseHandler = getUniverseHandler;
        _getGenerationHandler = getGenerationHandler;
        _getFinalStateHandler = getFinalStateHandler;
        _admissionGate = admissionGate;
        _linkGenerator = linkGenerator;
        _options = options;
    }

    [HttpPost]
    [RequestSizeLimit(1_048_576)]
    public async Task<IActionResult> CreateBoard([FromBody] UploadBoardRequest? request, CancellationToken ct)
    {
        if (request?.Cells is not { } cells)
        {
            return InvalidRequestProblem(new[] { BoardRequestValidator.MissingCellsError });
        }

        var errors = BoardRequestValidator.ValidateCells(cells, _options.Value);
        if (errors.Count > 0)
        {
            return InvalidRequestProblem(errors);
        }

        var rows = cells.Select(row => (IReadOnlyList<int>)row).ToList();
        var seed = Pattern.FromRows(rows);
        var id = UniverseId.NewId();

        await _createHandler.HandleAsync(new CreateUniverseCommand(id, seed), ct).ConfigureAwait(false);

        var response = new BoardCreatedResponse(id.ToString(), seed.Width, seed.Height, seed.Population, BuildBoardLinks(id.Value));
        return CreatedAtRoute("GetBoard", new { id = id.Value }, response);
    }

    [HttpGet("{id:guid}", Name = "GetBoard")]
    public async Task<IActionResult> GetBoard(Guid id, CancellationToken ct)
    {
        var result = await _getUniverseHandler.HandleAsync(new GetUniverseQuery(new UniverseId(id)), ct).ConfigureAwait(false);
        if (result.Status == ResultStatus.NotFound)
        {
            return BoardNotFoundProblem(id);
        }

        var view = result.Value;
        var response = new BoardResponse(
            view.Id.ToString(),
            view.Seed.Width,
            view.Seed.Height,
            view.Rule.Value,
            view.Topology.Value,
            view.CreatedAtUtc,
            PatternMapper.ToRows(view.Seed),
            BuildBoardLinks(id));

        return Ok(response);
    }

    [HttpGet("{id:guid}/generations/{n:int}", Name = "GetGeneration")]
    public async Task<IActionResult> GetGeneration(Guid id, int n, CancellationToken ct)
    {
        var validationErrors = BoardRequestValidator.ValidateGeneration(n, _options.Value);
        if (validationErrors.Count > 0)
        {
            return InvalidRequestProblem(validationErrors);
        }

        var lease = await _admissionGate.TryAcquireAsync(TimeSpan.Zero, ct).ConfigureAwait(false);
        if (lease is null)
        {
            return AdmissionRejectedProblem();
        }

        try
        {
            var result = await _getGenerationHandler.HandleAsync(new GetGenerationQuery(new UniverseId(id), n), ct).ConfigureAwait(false);
            if (result.Status == ResultStatus.NotFound)
            {
                return BoardNotFoundProblem(id);
            }

            return GenerationResult(result.Value, id);
        }
        finally
        {
            lease.Dispose();
        }
    }

    [HttpGet("{id:guid}/next", Name = "GetNextGeneration")]
    public async Task<IActionResult> GetNextGeneration(Guid id, CancellationToken ct)
    {
        var lease = await _admissionGate.TryAcquireAsync(TimeSpan.Zero, ct).ConfigureAwait(false);
        if (lease is null)
        {
            return AdmissionRejectedProblem();
        }

        try
        {
            var result = await _getGenerationHandler.HandleAsync(new GetGenerationQuery(new UniverseId(id), 1), ct).ConfigureAwait(false);
            if (result.Status == ResultStatus.NotFound)
            {
                return BoardNotFoundProblem(id);
            }

            return GenerationResult(result.Value, id);
        }
        finally
        {
            lease.Dispose();
        }
    }

    [HttpGet("{id:guid}/final", Name = "GetFinalState")]
    public async Task<IActionResult> GetFinalState(Guid id, CancellationToken ct)
    {
        var lease = await _admissionGate.TryAcquireAsync(TimeSpan.Zero, ct).ConfigureAwait(false);
        if (lease is null)
        {
            return AdmissionRejectedProblem();
        }

        try
        {
            var query = new GetFinalStateQuery(new UniverseId(id), _options.Value.FinalStateIterationBudget);
            var result = await _getFinalStateHandler.HandleAsync(query, ct).ConfigureAwait(false);
            if (result.Status == ResultStatus.NotFound)
            {
                return BoardNotFoundProblem(id);
            }

            var view = result.Value;
            if (view is not { Converged: true, Pattern: { } pattern })
            {
                return Problem(
                    type: "https://gameoflife.example/problems/final-state-not-converged",
                    title: "The board did not reach a final state within the iteration budget.",
                    statusCode: StatusCodes.Status422UnprocessableEntity,
                    detail: $"Examined {view.IterationsExamined} generations without finding a cycle.");
            }

            var response = new FinalStateResponse(
                id.ToString(),
                Converged: true,
                view.StabilizedAtGeneration,
                view.Period,
                pattern.Width,
                pattern.Height,
                pattern.Population,
                PatternMapper.ToRows(pattern),
                view.IterationsExamined,
                BuildFinalStateLinks(id));

            return Ok(response);
        }
        finally
        {
            lease.Dispose();
        }
    }

    private IActionResult GenerationResult(PatternView view, Guid id)
    {
        var etag = PatternMapper.ComputeETag(view.Pattern);
        Response.Headers.ETag = etag;
        Response.Headers.CacheControl = "public, max-age=31536000, immutable";

        if (Request.Headers.IfNoneMatch.Any(value => value == etag))
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        var response = new GenerationResponse(
            id.ToString(),
            view.Generation,
            view.Pattern.Width,
            view.Pattern.Height,
            view.Pattern.Population,
            PatternMapper.ToRows(view.Pattern),
            BuildGenerationLinks(id, view.Generation));

        return Ok(response);
    }

    private Dictionary<string, string> BuildBoardLinks(Guid id) => new()
    {
        ["self"] = Link("GetBoard", new { id }),
        ["next"] = Link("GetNextGeneration", new { id }),
        ["final"] = Link("GetFinalState", new { id }),
    };

    private Dictionary<string, string> BuildGenerationLinks(Guid id, int generation) => new()
    {
        ["self"] = Link("GetGeneration", new { id, n = generation }),
        ["next"] = Link("GetGeneration", new { id, n = generation + 1 }),
        ["final"] = Link("GetFinalState", new { id }),
        ["board"] = Link("GetBoard", new { id }),
    };

    private Dictionary<string, string> BuildFinalStateLinks(Guid id) => new()
    {
        ["self"] = Link("GetFinalState", new { id }),
        ["board"] = Link("GetBoard", new { id }),
    };

    // Links come from route templates only; a null here means a route name is wrong, which is a bug to surface, not a link to omit.
    private string Link(string routeName, object values) =>
        _linkGenerator.GetPathByName(HttpContext, routeName, values)
        ?? throw new InvalidOperationException($"Route '{routeName}' is not registered.");

    private static IActionResult BoardNotFoundProblem(Guid id) => new NotFoundObjectResult(new ProblemDetails
    {
        Type = "https://gameoflife.example/problems/board-not-found",
        Title = "Board not found.",
        Status = StatusCodes.Status404NotFound,
        Detail = $"No board exists with id '{id}'.",
    });

    private static IActionResult InvalidRequestProblem(IReadOnlyList<string> errors)
    {
        var problemDetails = new ValidationProblemDetails
        {
            Type = "https://gameoflife.example/problems/invalid-request",
            Title = "The request is invalid.",
            Status = StatusCodes.Status400BadRequest,
        };
        problemDetails.Errors["request"] = errors.ToArray();
        return new BadRequestObjectResult(problemDetails);
    }

    private IActionResult AdmissionRejectedProblem()
    {
        Response.Headers.RetryAfter = "1";
        return new ObjectResult(new ProblemDetails
        {
            Type = "https://gameoflife.example/problems/admission-rejected",
            Title = "The server is at capacity.",
            Status = StatusCodes.Status503ServiceUnavailable,
            Detail = "Too many evaluations are in progress. Retry shortly.",
        })
        {
            StatusCode = StatusCodes.Status503ServiceUnavailable,
        };
    }
}
