using FsCheck;
using FsCheck.Xunit;
using GameOfLife.Domain.Domain;
using GameOfLife.Domain.Rules;

namespace GameOfLife.UnitTests.Domain;

public sealed class DeterminismPropertyTests
{
    /// <summary>Builds a random rectangular 0/1 grid, bounded to a small size to keep tests fast.</summary>
    private static Pattern RandomPattern(int seed, int rawWidth, int rawHeight)
    {
        var width = Math.Abs(rawWidth) % 12 + 1;
        var height = Math.Abs(rawHeight) % 12 + 1;
        var random = new Random(seed);

        var rows = Enumerable.Range(0, height)
            .Select(_ => (IReadOnlyList<int>)[.. Enumerable.Range(0, width).Select(_ => random.Next(2))])
            .ToArray();

        return Pattern.FromRows(rows);
    }

    [Property]
    public bool GenerationAt_is_deterministic_for_the_same_seed_rule_and_topology(int seed, int rawWidth, int rawHeight,
        PositiveInt rawGeneration)
    {
        var pattern = RandomPattern(seed, rawWidth, rawHeight);
        var universe = new Universe(UniverseId.NewId(), pattern, RuleId.Standard, TopologyId.Bounded,
            DateTimeOffset.UnixEpoch);
        var generation = rawGeneration.Get % 20;

        var first = universe.GenerationAt(generation, CancellationToken.None);
        var second = universe.GenerationAt(generation, CancellationToken.None);

        return first.Equals(second);
    }

    [Property]
    public bool GenerationAt_zero_is_always_the_seed(int seed, int rawWidth, int rawHeight)
    {
        var pattern = RandomPattern(seed, rawWidth, rawHeight);
        var universe = new Universe(UniverseId.NewId(), pattern, RuleId.Standard, TopologyId.Bounded,
            DateTimeOffset.UnixEpoch);

        var generationZero = universe.GenerationAt(0, CancellationToken.None);

        return pattern.Equals(generationZero);
    }

    [Property]
    public bool NextGeneration_never_produces_more_population_than_cells_in_the_grid(int seed, int rawWidth,
        int rawHeight)
    {
        var pattern = RandomPattern(seed, rawWidth, rawHeight);
        var next = pattern.NextGeneration(StandardLifeRule.Instance, BoundedTopology.Instance, CancellationToken.None);

        return next.Population <= pattern.Width * pattern.Height;
    }

    [Property]
    public bool DetermineFate_is_deterministic(int seed, int rawWidth, int rawHeight)
    {
        var pattern = RandomPattern(seed, rawWidth, rawHeight);
        var universe = new Universe(UniverseId.NewId(), pattern, RuleId.Standard, TopologyId.Bounded,
            DateTimeOffset.UnixEpoch);

        var first = universe.DetermineFate(200, CancellationToken.None);
        var second = universe.DetermineFate(200, CancellationToken.None);

        return first.Equals(second);
    }
}
