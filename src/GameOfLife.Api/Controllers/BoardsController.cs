using FluentValidation;
using GameOfLife.Api.Contracts;
using GameOfLife.Api.Filters;
using GameOfLife.Api.Mapping;
using GameOfLife.Api.Options;
using GameOfLife.Api.Validation;
using GameOfLife.Application;
using GameOfLife.Application.CreateUniverse;
using GameOfLife.Application.GetFinalState;
using GameOfLife.Application.GetGeneration;
using GameOfLife.Application.GetUniverse;
using GameOfLife.Domain.Common;
using GameOfLife.Domain.Domain;
using GameOfLife.Domain.Rules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace GameOfLife.Api.Controllers;

/// <summary>
///     Translates HTTP onto the four use cases. Contains no business logic: it binds, validates the
///     boundary caps, calls a handler, and maps the Result onto a status code.
/// </summary>
[ApiController]
[Route("api/v1/boards")]
public sealed class BoardsController(
    ICommandHandler<CreateUniverseCommand> createHandler,
    IQueryHandler<GetUniverseQuery, UniverseView> getUniverseHandler,
    IQueryHandler<GetGenerationQuery, PatternView> getGenerationHandler,
    IQueryHandler<GetFinalStateQuery, FinalStateView> getFinalStateHandler,
    IValidator<UploadBoardRequest> uploadValidator,
    LinkGenerator linkGenerator,
    IOptions<GameOfLifeOptions> options)
    : ControllerBase
{
    /// <summary>Applied to the three endpoints that run the evolution loop; configured in Program.cs.</summary>
    public const string EvaluationPolicy = "evaluation";

    private readonly GameOfLifeOptions _options = options.Value;

    [HttpPost]
    // A cheap pre-parse guard only: a pretty-printed 256x256 board is roughly 580 KB of JSON, so 1 MB
    // leaves headroom. The authoritative cap is the cell count, which can only be checked after parsing.
    [RequestSizeLimit(1_048_576)]
    public async Task<IActionResult> CreateBoard([FromBody] UploadBoardRequest board, CancellationToken ct)
    {
        // Invoked here rather than by a filter. FluentValidation deprecated its MVC auto-validation
        // pipeline and does not ship a filter replacement, and AGENTS.md §1.3 puts binding and
        // validating in the controller anyway, so the explicit call is both supported and expected.
        // A null or absent body never reaches this point: the parameter is non-nullable, so MVC
        // rejects one during binding and the same factory shapes that response.
        var validationResult = await uploadValidator.ValidateAsync(board, ct);
        if (!validationResult.IsValid) return BadRequest(validationResult.ToDictionary().ToProblemDetails());

        var seed = Pattern.FromRows(board.Cells);
        var id = UniverseId.NewId();

        await createHandler.HandleAsync(new CreateUniverseCommand(id, seed, new RuleId(_options.DefaultRule)), ct);

        var response = new BoardCreatedResponse(id.ToString(), seed.Width, seed.Height, seed.Population,
            BuildBoardLinks(id.Value));
        return CreatedAtRoute("GetBoard", new { id = id.Value }, response);
    }

    [HttpGet("{id:guid}", Name = "GetBoard")]
    public async Task<IActionResult> GetBoard(Guid id, CancellationToken ct)
    {
        var result = await getUniverseHandler.HandleAsync(new GetUniverseQuery(new UniverseId(id)), ct);
        if (result.Status == ResultStatus.NotFound) return BoardNotFoundError(id);

        var view = result.Value;
        var response = new BoardResponse(
            view.Id.ToString(),
            view.Seed.Width,
            view.Seed.Height,
            view.Rule.Value,
            view.Topology.Value,
            view.CreatedAtUtc,
            view.Seed.ToRows(),
            BuildBoardLinks(id));

        return Ok(response);
    }

    [HttpGet("{id:guid}/generations/{n:int}", Name = "GetGeneration")]
    [EnableRateLimiting(EvaluationPolicy)]
    [GenerationETag]
    public async Task<IActionResult> GetGeneration(Guid id, int n, CancellationToken ct) =>
        // A single scalar bound from the route, so a validator class would be more ceremony than rule.
        n < 0 || n > _options.MaxGenerationsAhead
            ? BadRequest(new Dictionary<string, string[]>
            {
                ["n"] = [$"n must be between 0 and {_options.MaxGenerationsAhead}."]
            }.ToProblemDetails())
            : await GenerationAsync(id, n, ct);

    [HttpGet("{id:guid}/next", Name = "GetNextGeneration")]
    [EnableRateLimiting(EvaluationPolicy)]
    [GenerationETag(1)]
    public Task<IActionResult> GetNextGeneration(Guid id, CancellationToken ct) => GenerationAsync(id, 1, ct);

    [HttpGet("{id:guid}/final", Name = "GetFinalState")]
    [EnableRateLimiting(EvaluationPolicy)]
    public async Task<IActionResult> GetFinalState(Guid id, CancellationToken ct)
    {
        var result = await getFinalStateHandler.HandleAsync(
            new GetFinalStateQuery(
                new UniverseId(id),
                _options.FinalStateIterationBudget),
            ct);
        if (result.Status == ResultStatus.NotFound) return BoardNotFoundError(id);

        var view = result.Value;
        if (view.Stabilized is not { } cycle)
            return Problem(
                type: "https://gameoflife.example/problems/final-state-not-converged",
                title: "The board did not reach a final state within the iteration budget.",
                statusCode: StatusCodes.Status422UnprocessableEntity,
                detail: $"Examined {view.IterationsExamined} generations without finding a cycle.");

        var response = new FinalStateResponse(
            id.ToString(),
            cycle.AtGeneration,
            cycle.Period,
            cycle.Pattern.Width,
            cycle.Pattern.Height,
            cycle.Pattern.Population,
            cycle.Pattern.ToRows(),
            view.IterationsExamined,
            BuildFinalStateLinks(id));

        return Ok(response);
    }

    private async Task<IActionResult> GenerationAsync(Guid id, int generation, CancellationToken ct)
    {
        var result = await getGenerationHandler.HandleAsync(new GetGenerationQuery(new UniverseId(id), generation), ct);
        if (result.Status == ResultStatus.NotFound) return BoardNotFoundError(id);

        var view = result.Value;
        var response = new GenerationResponse(
            id.ToString(),
            view.Generation,
            view.Pattern.Width,
            view.Pattern.Height,
            view.Pattern.Population,
            view.Pattern.ToRows(),
            BuildGenerationLinks(id, view.Generation));

        return Ok(response);
    }

    private Dictionary<string, string> BuildBoardLinks(Guid id) =>
        new()
        {
            ["self"] = Link("GetBoard", new { id }),
            ["next"] = Link("GetNextGeneration", new { id }),
            ["final"] = Link("GetFinalState", new { id })
        };

    private Dictionary<string, string> BuildGenerationLinks(Guid id, int generation) =>
        new()
        {
            ["self"] = Link("GetGeneration", new { id, n = generation }),
            ["next"] = Link("GetGeneration", new { id, n = generation + 1 }),
            ["final"] = Link("GetFinalState", new { id }),
            ["board"] = Link("GetBoard", new { id })
        };

    private Dictionary<string, string> BuildFinalStateLinks(Guid id) =>
        new()
        {
            ["self"] = Link("GetFinalState", new { id }),
            ["board"] = Link("GetBoard", new { id })
        };

    // Links come from route templates only; a null here means a route name is wrong, which is a bug to surface, not a link to omit.
    private string Link(string routeName, object values) =>
        linkGenerator.GetPathByName(HttpContext, routeName, values)
        ?? throw new InvalidOperationException($"Route '{routeName}' is not registered.");

    private static NotFoundObjectResult BoardNotFoundError(Guid id) =>
        new(Errors.BoardNotFound(id));
}
