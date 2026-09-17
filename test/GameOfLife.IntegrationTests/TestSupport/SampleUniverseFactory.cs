using GameOfLife.Domain.Domain;

namespace GameOfLife.IntegrationTests.TestSupport;

internal static class SampleUniverseFactory
{
    public static Universe Create()
    {
        var seed = Pattern.FromRows(new List<IReadOnlyList<int>>
        {
            new List<int> { 0, 1, 0 },
            new List<int> { 0, 1, 0 },
            new List<int> { 0, 1, 0 },
        });

        return new Universe(UniverseId.NewId(), seed, RuleId.Standard, TopologyId.Bounded, DateTimeOffset.UtcNow);
    }
}
