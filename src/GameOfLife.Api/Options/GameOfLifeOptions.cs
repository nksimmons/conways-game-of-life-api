using System.ComponentModel.DataAnnotations;

namespace GameOfLife.Api.Options;

/// <summary>
///     Denial-of-service controls, not cosmetic validation: these bound the worst-case cost of a single
///     request. Validated on start, so a nonsensical value fails at boot rather than on the first request.
///     See docs/design.md §10.1 for the arithmetic behind the defaults.
/// </summary>
public sealed class GameOfLifeOptions
{
    public const string SectionName = "GameOfLife";

    [Range(1, int.MaxValue)] public int MaxWidth { get; init; } = 256;

    [Range(1, int.MaxValue)] public int MaxHeight { get; init; } = 256;

    [Range(1, int.MaxValue)] public int MaxCells { get; init; } = 65_536;

    [Range(0, int.MaxValue)] public int MaxGenerationsAhead { get; init; } = 1_000;

    [Range(1, int.MaxValue)] public int FinalStateIterationBudget { get; init; } = 5_000;

    /// <summary>0 means "derive it from <see cref="Environment.ProcessorCount" />."</summary>
    [Range(0, int.MaxValue)]
    public int MaxConcurrentEvaluations { get; init; }

    /// <summary>
    ///     Deliberately below the core count. The evolution loop is synchronous and CPU-bound, so permits
    ///     equal to <see cref="Environment.ProcessorCount" /> let saturation consume every core and stall
    ///     the request pipeline itself: measured at 10 permits on 10 cores, `/health/live` took 5.9 s and
    ///     would have failed a load balancer probe. Reserving cores fixed it (23 ms at one reserved, 16 ms
    ///     at two). Two is the default so the cheap endpoints sharing this process keep some margin.
    ///     See docs/design.md §8.4.
    /// </summary>
    public int ResolvedMaxConcurrentEvaluations => MaxConcurrentEvaluations > 0
        ? MaxConcurrentEvaluations
        : Math.Max(1, Environment.ProcessorCount - 2);
}