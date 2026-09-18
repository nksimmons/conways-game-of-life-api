using GameOfLife.Domain.Domain;
using GameOfLife.Domain.Rules;

namespace GameOfLife.IntegrationTests.TestSupport;

internal static class SampleUniverseFactory
{
    /// <summary>
    ///     Deliberately non-square and asymmetric. A square seed cannot detect a width and height swap
    ///     on the read path, because both dimensions pack to the same byte count and the reconstructed
    ///     pattern compares equal. This fixture is what makes the round-trip assertions orientation-aware.
    /// </summary>
    public static Universe Create() =>
        new(UniverseId.NewId(),
            Pattern.FromRows(new List<IReadOnlyList<int>>
            {
                new List<int> { 0, 1, 1, 0 },
                new List<int> { 0, 1, 0, 0 }
            }),
            RuleId.Standard,
            TopologyId.Bounded,
            DateTimeOffset.UtcNow);
}
