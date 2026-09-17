using System.Text.Json.Serialization;

namespace GameOfLife.Api.Contracts;

/// <summary>
/// On convergence, <see cref="Cells"/> is the pattern at <see cref="StabilizedAtGeneration"/>, the
/// generation where the detected cycle begins. On non-convergence, only <see cref="IterationsExamined"/>
/// is populated; see docs/design.md §6.3 for why that is a 422, not a 404 or 500.
/// </summary>
public sealed record FinalStateResponse(
    string BoardId,
    bool Converged,
    int? StabilizedAtGeneration,
    int? Period,
    int? Width,
    int? Height,
    int? Population,
    int[][]? Cells,
    int IterationsExamined,
    [property: JsonPropertyName("_links")] IReadOnlyDictionary<string, string> Links);
