using GameOfLife.Domain.Domain;
using GameOfLife.Domain.Rules;
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
    public void LifeRules_resolves_all_well_known_rule_ids()
    {
        Assert.Same(StandardLifeRule.Instance, RuleId.Standard.Resolve());
        Assert.Same(HighLifeRule.Instance, RuleId.HighLife.Resolve());
        Assert.Same(SeedsRule.Instance, RuleId.Seeds.Resolve());
        Assert.Same(LifeWithoutDeathRule.Instance, RuleId.LifeWithoutDeath.Resolve());
        Assert.Same(DayAndNightRule.Instance, RuleId.DayAndNight.Resolve());
        Assert.Same(MorleyRule.Instance, RuleId.Morley.Resolve());
        Assert.Same(TwoByTwoRule.Instance, RuleId.TwoByTwo.Resolve());
        Assert.Same(DiamoebaRule.Instance, RuleId.Diamoeba.Resolve());
        Assert.Same(FlockRule.Instance, RuleId.Flock.Resolve());
        Assert.Same(ThirtyFourLifeRule.Instance, RuleId.ThirtyFourLife.Resolve());
    }

    [Fact]
    public void HighLifeRule_implements_B36S23()
    {
        Assert.True(HighLifeRule.Instance.NextState(false, 3));
        Assert.True(HighLifeRule.Instance.NextState(false, 6));
        Assert.False(HighLifeRule.Instance.NextState(false, 2));
        Assert.True(HighLifeRule.Instance.NextState(true, 2));
        Assert.True(HighLifeRule.Instance.NextState(true, 3));
        Assert.False(HighLifeRule.Instance.NextState(true, 4));
    }

    [Fact]
    public void SeedsRule_implements_B2S()
    {
        Assert.True(SeedsRule.Instance.NextState(false, 2));
        Assert.False(SeedsRule.Instance.NextState(false, 3));
        Assert.False(SeedsRule.Instance.NextState(true, 2));
        Assert.False(SeedsRule.Instance.NextState(true, 0));
    }

    [Fact]
    public void LifeWithoutDeathRule_implements_B3S012345678()
    {
        Assert.True(LifeWithoutDeathRule.Instance.NextState(false, 3));
        Assert.False(LifeWithoutDeathRule.Instance.NextState(false, 2));
        for (var n = 0; n <= 8; n++) Assert.True(LifeWithoutDeathRule.Instance.NextState(true, n));
    }

    [Fact]
    public void DayAndNightRule_implements_B3678S34678()
    {
        Assert.True(DayAndNightRule.Instance.NextState(false, 3));
        Assert.True(DayAndNightRule.Instance.NextState(false, 6));
        Assert.True(DayAndNightRule.Instance.NextState(false, 7));
        Assert.True(DayAndNightRule.Instance.NextState(false, 8));
        Assert.False(DayAndNightRule.Instance.NextState(false, 2));
        Assert.True(DayAndNightRule.Instance.NextState(true, 3));
        Assert.True(DayAndNightRule.Instance.NextState(true, 4));
        Assert.True(DayAndNightRule.Instance.NextState(true, 6));
        Assert.True(DayAndNightRule.Instance.NextState(true, 7));
        Assert.True(DayAndNightRule.Instance.NextState(true, 8));
        Assert.False(DayAndNightRule.Instance.NextState(true, 2));
    }

    [Fact]
    public void MorleyRule_implements_B368S245()
    {
        Assert.True(MorleyRule.Instance.NextState(false, 3));
        Assert.True(MorleyRule.Instance.NextState(false, 6));
        Assert.True(MorleyRule.Instance.NextState(false, 8));
        Assert.False(MorleyRule.Instance.NextState(false, 4));
        Assert.True(MorleyRule.Instance.NextState(true, 2));
        Assert.True(MorleyRule.Instance.NextState(true, 4));
        Assert.True(MorleyRule.Instance.NextState(true, 5));
        Assert.False(MorleyRule.Instance.NextState(true, 3));
    }

    [Fact]
    public void TwoByTwoRule_implements_B36S125()
    {
        Assert.True(TwoByTwoRule.Instance.NextState(false, 3));
        Assert.True(TwoByTwoRule.Instance.NextState(false, 6));
        Assert.False(TwoByTwoRule.Instance.NextState(false, 2));
        Assert.True(TwoByTwoRule.Instance.NextState(true, 1));
        Assert.True(TwoByTwoRule.Instance.NextState(true, 2));
        Assert.True(TwoByTwoRule.Instance.NextState(true, 5));
        Assert.False(TwoByTwoRule.Instance.NextState(true, 3));
    }

    [Fact]
    public void DiamoebaRule_implements_B35678S5678()
    {
        Assert.True(DiamoebaRule.Instance.NextState(false, 3));
        Assert.True(DiamoebaRule.Instance.NextState(false, 5));
        Assert.False(DiamoebaRule.Instance.NextState(false, 2));
        Assert.True(DiamoebaRule.Instance.NextState(true, 5));
        Assert.True(DiamoebaRule.Instance.NextState(true, 8));
        Assert.False(DiamoebaRule.Instance.NextState(true, 2));
    }

    [Fact]
    public void FlockRule_implements_B3S12()
    {
        Assert.True(FlockRule.Instance.NextState(false, 3));
        Assert.False(FlockRule.Instance.NextState(false, 2));
        Assert.True(FlockRule.Instance.NextState(true, 1));
        Assert.True(FlockRule.Instance.NextState(true, 2));
        Assert.False(FlockRule.Instance.NextState(true, 3));
    }

    [Fact]
    public void ThirtyFourLifeRule_implements_B34S34()
    {
        Assert.True(ThirtyFourLifeRule.Instance.NextState(false, 3));
        Assert.True(ThirtyFourLifeRule.Instance.NextState(false, 4));
        Assert.False(ThirtyFourLifeRule.Instance.NextState(false, 2));
        Assert.True(ThirtyFourLifeRule.Instance.NextState(true, 3));
        Assert.True(ThirtyFourLifeRule.Instance.NextState(true, 4));
        Assert.False(ThirtyFourLifeRule.Instance.NextState(true, 2));
    }

    [Fact]
    public void LifeRules_throws_for_an_unknown_rule_id() =>
        Assert.Throws<InvalidOperationException>(() => new RuleId("nonexistent").Resolve());

    [Fact]
    public void Seeds_is_not_an_alias_for_survival_with_zero_neighbors()
    {
        var rule = new RuleId("B2/S0");

        Assert.False(rule.IsSupported());
        Assert.Throws<InvalidOperationException>(() => rule.Resolve());
        Assert.True(RuleId.Seeds.IsSupported());
        Assert.False(RuleId.Seeds.Resolve().NextState(true, 0));
    }

    [Theory]
    [InlineData("B3/S23")]
    [InlineData("B36/S23")]
    [InlineData("B2/S")]
    [InlineData("B3/S012345678")]
    [InlineData("B3678/S34678")]
    [InlineData("B368/S245")]
    [InlineData("B36/S125")]
    [InlineData("B35678/S5678")]
    [InlineData("B3/S12")]
    [InlineData("B34/S34")]
    public void Supported_rules_match_their_notation_for_every_cell_state_and_neighbor_count(string notation)
    {
        var id = new RuleId(notation);
        var counts = notation.Split('/');
        Assert.True(id.IsSupported());
        var rule = id.Resolve();

        foreach (var alive in new[] { false, true })
            foreach (var count in Enumerable.Range(0, 9))
                Assert.Equal(counts[alive ? 1 : 0].Contains((char)('0' + count)), rule.NextState(alive, count));
    }
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
