using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GameOfLife.Api.Validation;

/// <summary>
/// Builds the single RFC 7807 shape used for every 400 in this API.
/// <para>
/// Two things produce invalid-request errors: model binding, which fails before an action runs, and
/// FluentValidation, which the action invokes. They previously produced different bodies, with
/// different <c>type</c> URIs, titles, and error keys, so a client could not handle them uniformly.
/// Routing both through here fixes that, and strips the binder's messages, which otherwise leak
/// framework internals such as <c>System.Int32</c> and JSON byte offsets into the response.
/// </para>
/// </summary>
public static class ValidationProblems
{
    public const string ProblemType = "https://gameoflife.example/problems/invalid-request";
    public const string ProblemTitle = "The request is invalid.";

    private const string UnreadableValue = "The value supplied is not valid for this field.";

    public static ValidationProblemDetails Create(IDictionary<string, string[]> errors)
    {
        var problem = new ValidationProblemDetails(CamelCaseKeys(errors))
        {
            Type = ProblemType,
            Title = ProblemTitle,
            Status = StatusCodes.Status400BadRequest,
        };

        return problem;
    }

    /// <summary>Converts model-binding failures, replacing the binder's internals-bearing text.</summary>
    public static ValidationProblemDetails Create(ModelStateDictionary modelState)
    {
        // Model binding reports a missing body under an empty key, which is not something a client can
        // act on, and normalising it can collide with an existing key, so the messages are grouped.
        var errors = modelState
            .Where(entry => entry.Value is { Errors.Count: > 0 })
            .GroupBy(entry => string.IsNullOrEmpty(entry.Key) ? "body" : entry.Key, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.SelectMany(entry => entry.Value!.Errors.Select(Describe)).Distinct().ToArray(),
                StringComparer.Ordinal);

        return Create(errors);
    }

    // A binding error carries either an exception or a message written for a developer. Neither is
    // safe to echo: the JSON binder's text names CLR types and byte positions. The property path in
    // the dictionary key already tells the client which field to fix.
    private static string Describe(ModelError error) =>
        error.Exception is not null || string.IsNullOrWhiteSpace(error.ErrorMessage) || IsBinderText(error.ErrorMessage)
            ? UnreadableValue
            : error.ErrorMessage;

    private static bool IsBinderText(string message) =>
        message.Contains("System.", StringComparison.Ordinal)
        || message.Contains("LineNumber", StringComparison.Ordinal)
        || message.Contains("BytePositionInLine", StringComparison.Ordinal);

    private static Dictionary<string, string[]> CamelCaseKeys(IDictionary<string, string[]> errors) =>
        errors.ToDictionary(pair => ToCamelCase(pair.Key), pair => pair.Value, StringComparer.Ordinal);

    // FluentValidation reports "Cells"; the binder reports "$.cells[0][0]". Both become camelCase so
    // the keys line up with the JSON the client actually sent.
    private static string ToCamelCase(string key) =>
        string.IsNullOrEmpty(key) ? key : JsonNamingPolicy.CamelCase.ConvertName(key);
}
