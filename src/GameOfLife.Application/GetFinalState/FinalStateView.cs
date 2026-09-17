using GameOfLife.Domain.Domain;

namespace GameOfLife.Application.GetFinalState;

/// <summary>
/// The outcome of searching for a universe's fate. When <see cref="Converged"/> is false, the search
/// exhausted its budget rather than proving the pattern never settles; every finite universe does.
/// </summary>
public sealed record FinalStateView(
    UniverseId UniverseId,
    bool Converged,
    int? StabilizedAtGeneration,
    int? Period,
    Pattern? Pattern,
    int IterationsExamined);
