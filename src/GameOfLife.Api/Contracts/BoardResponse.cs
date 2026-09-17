using System.Text.Json.Serialization;

namespace GameOfLife.Api.Contracts;

public sealed record BoardResponse(
    string BoardId,
    int Width,
    int Height,
    string Rule,
    string Topology,
    DateTimeOffset CreatedAtUtc,
    int[][] Cells,
    [property: JsonPropertyName("_links")] IReadOnlyDictionary<string, string> Links);
