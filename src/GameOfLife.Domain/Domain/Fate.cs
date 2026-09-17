namespace GameOfLife.Domain.Domain;

/// <summary>
/// What a pattern's search for stability turned up: either a cycle was found within the budget,
/// or the budget ran out first. There is no third case; every finite universe eventually cycles.
/// </summary>
public abstract record Fate
{
    private Fate()
    {
    }

    /// <summary>A cycle was found. <paramref name="Period"/> of 1 is a still life; greater is an oscillator.</summary>
    public sealed record Stabilized(int AtGeneration, int Period) : Fate;

    /// <summary>No cycle was found within <paramref name="GenerationsExamined"/> generations.</summary>
    public sealed record Undetermined(int GenerationsExamined) : Fate;
}
