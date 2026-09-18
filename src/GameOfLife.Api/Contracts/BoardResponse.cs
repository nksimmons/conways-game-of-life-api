using System.Text.Json.Serialization;

namespace GameOfLife.Api.Contracts;

/// <summary>
///     The initial state and configuration of a board: seed pattern, evolution rule, topology, and hypermedia links.
/// </summary>
public sealed record BoardResponse(
    string BoardId,
    int Width,
    int Height,
    string Rule,
    string Topology,
    DateTimeOffset CreatedAtUtc,
    int[][] Cells,
    [property: JsonPropertyName("_links")] IReadOnlyDictionary<string, string> Links);