using System.Text.Json.Serialization;

namespace GameOfLife.Api.Contracts;

/// <summary>
///     The response returned after creating a board, carrying its assigned id and hypermedia links to the
///     subsequent projection endpoints.
/// </summary>
public sealed record BoardCreatedResponse(
    string BoardId,
    int Width,
    int Height,
    int Population,
    [property: JsonPropertyName("_links")] IReadOnlyDictionary<string, string> Links);