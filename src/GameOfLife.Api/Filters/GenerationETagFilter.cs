using GameOfLife.Api.Options;
using GameOfLife.Application;
using GameOfLife.Application.GetUniverse;
using GameOfLife.Domain.Common;
using GameOfLife.Domain.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace GameOfLife.Api.Filters;

/// <summary>
///     Decorates an action that returns a generation representation with deterministic ETag and
///     immutable cache headers. For matching <c>If-None-Match</c> requests, it verifies board existence
///     via a fast point lookup and returns <c>304 Not Modified</c> without executing the evolution loop.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class GenerationETagAttribute : TypeFilterAttribute
{
    public GenerationETagAttribute() : base(typeof(GenerationETagFilter))
    {
    }

    public GenerationETagAttribute(int generation) : base(typeof(GenerationETagFilter))
    {
        Arguments = [generation];
    }
}

/// <summary>
///     Async action filter evaluating conditional <c>If-None-Match</c> headers against deterministic ETags,
///     returning 304 Not Modified on cache hit, and setting immutable cache control headers.
/// </summary>
public sealed class GenerationETagFilter(
    IQueryHandler<GetUniverseQuery, UniverseView> getUniverseHandler,
    IOptions<GameOfLifeOptions> options,
    int generation = -1)
    : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ActionArguments.TryGetValue("id", out var idObj) || idObj is not Guid id)
        {
            await next();
            return;
        }

        var targetGeneration = generation >= 0
            ? generation
            : context.ActionArguments.TryGetValue("n", out var nObj) && nObj is int nVal
                ? nVal
                : -1;

        if (targetGeneration < 0 || targetGeneration > options.Value.MaxGenerationsAhead)
        {
            await next();
            return;
        }

        // Derived from identity, not from content. The representation is a pure function of the board
        // id and the generation index, both immutable, so the validator can be computed without
        // evolving anything, which is what lets a conditional request skip the loop entirely.
        var etag = new EntityTagHeaderValue($"\"v1-{id}-{targetGeneration}\"");
        var requestHeaders = context.HttpContext.Request.GetTypedHeaders();
        var ifNoneMatch = requestHeaders.IfNoneMatch;

        if (ifNoneMatch.Any(tag => tag.Equals(EntityTagHeaderValue.Any) || tag.Compare(etag, useStrongComparison: false)))
        {
            // Still confirm the board exists, so a fabricated validator gets a 404 rather than a
            // spurious 304. This is a point lookup, not an evolution.
            var ct = context.HttpContext.RequestAborted;
            var universeResult = await getUniverseHandler.HandleAsync(new GetUniverseQuery(new UniverseId(id)), ct);
            if (universeResult.Status == ResultStatus.NotFound)
            {
                context.Result = new NotFoundObjectResult(new ProblemDetails
                {
                    Type = "https://gameoflife.example/problems/board-not-found",
                    Title = "Board not found.",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"No board exists with id '{id}'."
                });
                return;
            }

            SetCacheHeaders(context.HttpContext.Response, etag);
            context.Result = new StatusCodeResult(StatusCodes.Status304NotModified);
            return;
        }

        var executedContext = await next();

        if (executedContext.Result is OkObjectResult)
        {
            SetCacheHeaders(executedContext.HttpContext.Response, etag);
        }
    }

    private static void SetCacheHeaders(HttpResponse response, EntityTagHeaderValue etag)
    {
        var typedHeaders = response.GetTypedHeaders();
        typedHeaders.ETag = etag;
        typedHeaders.CacheControl = new CacheControlHeaderValue
        {
            Public = true,
            MaxAge = TimeSpan.FromSeconds(31536000),
            Extensions = { new NameValueHeaderValue("immutable") }
        };
    }
}
