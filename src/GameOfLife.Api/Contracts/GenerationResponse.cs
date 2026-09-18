using System.Text.Json.Serialization;

namespace GameOfLife.Api.Contracts;

/// <summary>
///     The state of a board at generation <see cref="Generation" />, including dimensions, population count,
///     and cell grid.
/// </summary>
public sealed record GenerationResponse(
    string BoardId,
    int Generation,
    int Width,
    int Height,
    int Population,
    int[][] Cells,
    [property: JsonPropertyName("_links")] IReadOnlyDictionary<string, string> Links);