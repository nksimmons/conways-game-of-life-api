using System.Text.Json.Serialization;

namespace GameOfLife.Api.Contracts;

public sealed record GenerationResponse(
    string BoardId,
    int Generation,
    int Width,
    int Height,
    int Population,
    int[][] Cells,
    [property: JsonPropertyName("_links")] IReadOnlyDictionary<string, string> Links);
