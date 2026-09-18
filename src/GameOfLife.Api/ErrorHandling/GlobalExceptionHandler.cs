using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GameOfLife.Api.ErrorHandling;

/// <summary>Turns anything a controller didn't anticipate into ProblemDetails, never a stack trace.</summary>
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        if (exception is OperationCanceledException)
        {
            return true;
        }

        // Kestrel raises this for protocol-level rejections, most relevantly an oversized body (413).
        // It is an anticipated client error, so it keeps its own status code rather than becoming a 500.
        if (exception is BadHttpRequestException badRequest)
        {
            await WriteProblemAsync(
                httpContext,
                badRequest.StatusCode,
                "https://gameoflife.example/problems/bad-request",
                badRequest.StatusCode == StatusCodes.Status413PayloadTooLarge
                    ? "The request body is too large."
                    : "The request could not be processed.",
                ct);
            return true;
        }

        logger.LogError(
            exception,
            "Unhandled exception processing {Method} {Path}",
            httpContext.Request.Method,
            httpContext.Request.Path);

        await WriteProblemAsync(
            httpContext,
            StatusCodes.Status500InternalServerError,
            "https://gameoflife.example/problems/internal-error",
            "An unexpected error occurred.",
            ct);

        return true;
    }

    private static Task WriteProblemAsync(HttpContext httpContext, int statusCode, string type, string title, CancellationToken ct)
    {
        httpContext.Response.StatusCode = statusCode;
        return httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails { Type = type, Title = title, Status = statusCode },
            options: null,
            contentType: "application/problem+json",
            ct);
    }
}
