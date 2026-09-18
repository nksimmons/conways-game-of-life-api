using GameOfLife.Domain.Common;
using GameOfLife.Domain.Domain;
using GameOfLife.Application;
using GameOfLife.Application.CreateUniverse;
using GameOfLife.Application.GetFinalState;
using GameOfLife.Application.GetGeneration;
using GameOfLife.Application.GetUniverse;
using GameOfLife.Api.Contracts;
using GameOfLife.Api.Mapping;
using GameOfLife.Api.Options;
using GameOfLife.Api.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
    /// <summary>Applied to the three endpoints that run the evolution loop; configured in Program.cs.</summary>
    public const string EvaluationPolicy = "evaluation";

    private readonly ICommandHandler<CreateUniverseCommand> _createHandler;
    private readonly IQueryHandler<GetUniverseQuery, UniverseView> _getUniverseHandler;
    private readonly IQueryHandler<GetGenerationQuery, PatternView> _getGenerationHandler;
    private readonly IQueryHandler<GetFinalStateQuery, FinalStateView> _getFinalStateHandler;
    private readonly LinkGenerator _linkGenerator;
    private readonly GameOfLifeOptions _options;

    public BoardsController(
        ICommandHandler<CreateUniverseCommand> createHandler,
        IQueryHandler<GetUniverseQuery, UniverseView> getUniverseHandler,
        IQueryHandler<GetGenerationQuery, PatternView> getGenerationHandler,
        IQueryHandler<GetFinalStateQuery, FinalStateView> getFinalStateHandler,
        LinkGenerator linkGenerator,
        IOptions<GameOfLifeOptions> options)
    {
        _createHandler = createHandler;
        _getUniverseHandler = getUniverseHandler;
        _getGenerationHandler = getGenerationHandler;
        _getFinalStateHandler = getFinalStateHandler;
        _linkGenerator = linkGenerator;
        _options = options.Value;
    }

    [HttpPost]
    // A cheap pre-parse guard only: a pretty-printed 256x256 board is roughly 580 KB of JSON, so 1 MB
    // leaves headroom. The authoritative cap is the cell count, which can only be checked after parsing.
    [RequestSizeLimit(1_048_576)]
    public async Task<IActionResult> CreateBoard([FromBody] UploadBoardRequest? request, CancellationToken ct)
    {
        if (request?.Cells is not { } cells)
        {
            return InvalidRequestProblem(new[] { BoardRequestValidator.MissingCellsError });
        }

        var errors = BoardRequestValidator.ValidateCells(cells, _options);
        if (errors.Count > 0)
        {
            return InvalidRequestProblem(errors);
        }

        var seed = Pattern.FromRows(cells);
        var id = UniverseId.NewId();

        await _createHandler.HandleAsync(new CreateUniverseCommand(id, seed), ct);

        var response = new BoardCreatedResponse(id.ToString(), seed.Width, seed.Height, seed.Population, BuildBoardLinks(id.Value));
        return CreatedAtRoute("GetBoard", new { id = id.Value }, response);
    }

    [HttpGet("{id:guid}", Name = "GetBoard")]
    public async Task<IActionResult> GetBoard(Guid id, CancellationToken ct)
    {
        var result = await _getUniverseHandler.HandleAsync(new GetUniverseQuery(new UniverseId(id)), ct);
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
    [EnableRateLimiting(EvaluationPolicy)]
    public async Task<IActionResult> GetGeneration(Guid id, int n, CancellationToken ct)
    {
        var errors = BoardRequestValidator.ValidateGeneration(n, _options);
        return errors.Count > 0
            ? InvalidRequestProblem(errors)
            : await GenerationAsync(id, n, ct);
    }

    [HttpGet("{id:guid}/next", Name = "GetNextGeneration")]
    [EnableRateLimiting(EvaluationPolicy)]
    public Task<IActionResult> GetNextGeneration(Guid id, CancellationToken ct) => GenerationAsync(id, 1, ct);

    [HttpGet("{id:guid}/final", Name = "GetFinalState")]
    [EnableRateLimiting(EvaluationPolicy)]
    public async Task<IActionResult> GetFinalState(Guid id, CancellationToken ct)
    {
        var query = new GetFinalStateQuery(new UniverseId(id), _options.FinalStateIterationBudget);
        var result = await _getFinalStateHandler.HandleAsync(query, ct);
        if (result.Status == ResultStatus.NotFound)
        {
            return BoardNotFoundProblem(id);
        }

        var view = result.Value;
        if (view.Stabilized is not { } cycle)
        {
            return Problem(
                type: "https://gameoflife.example/problems/final-state-not-converged",
                title: "The board did not reach a final state within the iteration budget.",
                statusCode: StatusCodes.Status422UnprocessableEntity,
                detail: $"Examined {view.IterationsExamined} generations without finding a cycle.");
        }

        var response = new FinalStateResponse(
            id.ToString(),
            cycle.AtGeneration,
            cycle.Period,
            cycle.Pattern.Width,
            cycle.Pattern.Height,
            cycle.Pattern.Population,
            PatternMapper.ToRows(cycle.Pattern),
            view.IterationsExamined,
            BuildFinalStateLinks(id));

        return Ok(response);
    }

    private async Task<IActionResult> GenerationAsync(Guid id, int generation, CancellationToken ct)
    {
        // Derived from identity, not from content. The representation is a pure function of the board
        // id and the generation index, both immutable, so the validator can be computed without
        // evolving anything, which is what lets a conditional request skip the loop entirely. A
        // content hash cannot do that, and it also collides: a blinker at generations 0 and 2 has the
        // same cells but is a different representation, with a different `generation` and `_links`.
        // The `v1` prefix is the representation version, so a change to the response shape invalidates
        // previously issued validators.
        var etag = $"\"v1-{id}-{generation}\"";

        if (Request.Headers.IfNoneMatch.Any(value => value == etag))
        {
            // Still confirm the board exists, so a fabricated validator gets a 404 rather than a
            // spurious 304. This is a point lookup, not an evolution.
            var known = await _getUniverseHandler.HandleAsync(new GetUniverseQuery(new UniverseId(id)), ct);
            if (known.Status == ResultStatus.NotFound)
            {
                return BoardNotFoundProblem(id);
            }

            SetGenerationCacheHeaders(etag);
            return StatusCode(StatusCodes.Status304NotModified);
        }

        var result = await _getGenerationHandler.HandleAsync(new GetGenerationQuery(new UniverseId(id), generation), ct);
        if (result.Status == ResultStatus.NotFound)
        {
            return BoardNotFoundProblem(id);
        }

        var view = result.Value;
        SetGenerationCacheHeaders(etag);

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

    private void SetGenerationCacheHeaders(string etag)
    {
        Response.Headers.ETag = etag;
        Response.Headers.CacheControl = "public, max-age=31536000, immutable";
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
}
