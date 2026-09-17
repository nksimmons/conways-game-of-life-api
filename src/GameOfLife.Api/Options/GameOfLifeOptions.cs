namespace GameOfLife.Api.Options;

/// <summary>
/// Denial-of-service controls, not cosmetic validation: these bound the worst-case cost of a single
/// request. See docs/design.md §10.1 for the arithmetic behind the defaults.
/// </summary>
public sealed class GameOfLifeOptions
{
    public const string SectionName = "GameOfLife";

    public int MaxWidth { get; set; } = 256;

    public int MaxHeight { get; set; } = 256;

    public int MaxCells { get; set; } = 65_536;

    public int MaxGenerationsAhead { get; set; } = 1_000;

    public int FinalStateIterationBudget { get; set; } = 5_000;

    /// <summary>0 means "use <see cref="Environment.ProcessorCount"/>."</summary>
    public int MaxConcurrentEvaluations { get; set; }

    public int ResolvedMaxConcurrentEvaluations => MaxConcurrentEvaluations > 0 ? MaxConcurrentEvaluations : Environment.ProcessorCount;
}
