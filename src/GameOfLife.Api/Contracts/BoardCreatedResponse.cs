using System.Text.Json.Serialization;

namespace GameOfLife.Api.Contracts;

public sealed record BoardCreatedResponse(
    string BoardId,
    int Width,
    int Height,
    int Population,
    [property: JsonPropertyName("_links")] IReadOnlyDictionary<string, string> Links);
