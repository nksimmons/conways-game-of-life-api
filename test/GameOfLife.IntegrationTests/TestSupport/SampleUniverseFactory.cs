using GameOfLife.Domain.Domain;

namespace GameOfLife.IntegrationTests.TestSupport;

internal static class SampleUniverseFactory
{
    public static Universe Create() =>
        new(UniverseId.NewId(),
            Pattern.FromRows(new List<IReadOnlyList<int>>
            {
                new List<int> { 0, 1, 0 },
                new List<int> { 0, 1, 0 },
                new List<int> { 0, 1, 0 }
            }),
            RuleId.Standard,
            TopologyId.Bounded,
            DateTimeOffset.UtcNow);
}