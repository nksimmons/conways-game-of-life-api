using GameOfLife.Domain.Domain;
using GameOfLife.UnitTests.TestSupport;

namespace GameOfLife.UnitTests.Domain;

public sealed class LifeRuleTests
{
    [Theory]
    [InlineData(true, 0, false)]
    [InlineData(true, 1, false)]
    [InlineData(true, 2, true)]
    [InlineData(true, 3, true)]
    [InlineData(true, 4, false)]
    [InlineData(true, 5, false)]
    [InlineData(true, 6, false)]
    [InlineData(true, 7, false)]
    [InlineData(true, 8, false)]
    [InlineData(false, 0, false)]
    [InlineData(false, 1, false)]
    [InlineData(false, 2, false)]
    [InlineData(false, 3, true)]
    [InlineData(false, 4, false)]
    [InlineData(false, 5, false)]
    [InlineData(false, 6, false)]
    [InlineData(false, 7, false)]
    [InlineData(false, 8, false)]
    public void StandardLifeRule_implements_B3S23(bool alive, int liveNeighbors, bool expected) =>
        Assert.Equal(expected, StandardLifeRule.Instance.NextState(alive, liveNeighbors));

    [Fact]
    public void LifeRules_resolves_the_standard_rule_id() =>
        Assert.Same(StandardLifeRule.Instance, RuleId.Standard.Resolve());

    [Fact]
    public void LifeRules_throws_for_an_unknown_rule_id() =>
        Assert.Throws<InvalidOperationException>(() => new RuleId("nonexistent").Resolve());
}

public sealed class TopologyTests
{
    [Fact]
    public void BoundedTopology_treats_off_grid_neighbors_as_dead()
    {
        var corner = PatternTestHelper.FromAscii(
            "OO",
            "OO");

        // The top-left cell has only 3 in-bounds neighbours, all alive; the rest are off-grid and dead.
        var count = BoundedTopology.Instance.CountLiveNeighbors(corner, 0, 0);

        Assert.Equal(3, count);
    }

    [Fact]
    public void ToroidalTopology_wraps_across_edges()
    {
        // A single live cell at (0,0) on a 3x3 torus has itself... no, we place cells at the far
        // corners so that, wrapped, they are all adjacent to (0,0).
        var pattern = PatternTestHelper.FromAscii(
            "..O",
            "...",
            "O..");

        // (0,2) is directly left-adjacent (wrapping) of (0,0); (2,0) is directly above (wrapping).
        var count = ToroidalTopology.Instance.CountLiveNeighbors(pattern, 0, 0);

        Assert.Equal(2, count);
    }

    [Fact]
    public void Topologies_resolves_both_well_known_topologies()
    {
        Assert.Same(BoundedTopology.Instance, TopologyId.Bounded.Resolve());
        Assert.Same(ToroidalTopology.Instance, TopologyId.Toroidal.Resolve());
    }

    [Fact]
    public void Topologies_throws_for_an_unknown_topology_id() =>
        Assert.Throws<InvalidOperationException>(() => new TopologyId("nonexistent").Resolve());
}
