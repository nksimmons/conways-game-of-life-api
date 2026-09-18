using System.Text.Json.Serialization;

namespace GameOfLife.Api.Contracts;

/// <summary>
/// A converged final state: <see cref="Cells"/> is the pattern at <see cref="StabilizedAtGeneration"/>,
/// where the detected cycle begins. Non-convergence is a 422 rather than a body with a false flag; see
/// docs/design.md §6.3, so every field here is populated.
/// </summary>
public sealed record FinalStateResponse(
    string BoardId,
    int StabilizedAtGeneration,
    int Period,
    int Width,
    int Height,
    int Population,
    int[][] Cells,
    int IterationsExamined,
    [property: JsonPropertyName("_links")] IReadOnlyDictionary<string, string> Links);
