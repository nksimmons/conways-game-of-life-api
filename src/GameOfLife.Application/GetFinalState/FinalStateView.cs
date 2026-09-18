using GameOfLife.Domain.Domain;

namespace GameOfLife.Application.GetFinalState;

/// <summary>
///     The outcome of searching for a universe's fate. <see cref="Stabilized" /> is null when the search
///     exhausted its budget, which is not the same as proving the pattern never settles; every finite
///     universe does. The three values that only exist on convergence travel together so they cannot disagree.
/// </summary>
public sealed record FinalStateView(
    UniverseId UniverseId,
    int IterationsExamined,
    FinalStateView.Cycle? Stabilized)
{
    /// <summary>Where the detected cycle begins, how long it is, and the pattern at its start.</summary>
    public sealed record Cycle(int AtGeneration, int Period, Pattern Pattern);
}