using GameOfLife.Domain.Domain;
using GameOfLife.UnitTests.TestSupport;

namespace GameOfLife.UnitTests.Domain;

/// <summary>
/// Tests built from the published pattern oracles (docs/design.md §6.4). Blinker and Glider in
/// particular catch "NaiveLife" (updating cells in reading order instead of simultaneously), the
/// most common implementation bug in this problem.
/// </summary>
public sealed class UniverseOracleTests
{
    private static Universe CreateUniverse(Pattern seed, TopologyId? topology = null) =>
        new(UniverseId.NewId(), seed, RuleId.Standard, topology ?? TopologyId.Bounded, DateTimeOffset.UnixEpoch);

    [Fact]
    public void Block_is_a_still_life()
    {
        var seed = PatternTestHelper.FromAscii(
            "OO",
            "OO");

        var universe = CreateUniverse(seed);
        var fate = universe.DetermineFate(iterationBudget: 10, CancellationToken.None);

        var stabilized = Assert.IsType<Fate.Stabilized>(fate);
        Assert.Equal(0, stabilized.AtGeneration);
        Assert.Equal(1, stabilized.Period);
    }

    [Theory]
    [InlineData([new[] { "...", "OOO", "..." }])] // Blinker
    [InlineData([new[] { "....", ".OOO", "OOO.", "...." }])] // Toad
    public void Oscillator_has_period_2(string[] rows)
    {
        var seed = PatternTestHelper.FromAscii(rows);
        var universe = CreateUniverse(seed);

        var fate = universe.DetermineFate(iterationBudget: 10, CancellationToken.None);

        var stabilized = Assert.IsType<Fate.Stabilized>(fate);
        Assert.Equal(0, stabilized.AtGeneration);
        Assert.Equal(2, stabilized.Period);
    }

    [Fact]
    public void Beacon_has_period_2()
    {
        var seed = PatternTestHelper.FromAscii(
            "OO..",
            "OO..",
            "..OO",
            "..OO");

        var universe = CreateUniverse(seed);
        var fate = universe.DetermineFate(iterationBudget: 10, CancellationToken.None);

        var stabilized = Assert.IsType<Fate.Stabilized>(fate);
        Assert.Equal(0, stabilized.AtGeneration);
        Assert.Equal(2, stabilized.Period);
    }

    [Fact]
    public void Pulsar_has_period_3()
    {
        // The pulsar's own bounding box touches all four of its edges, but transient generations
        // between the repeats extend a cell or two beyond it, so it needs padding on a bounded grid
        // just as Glider and Diehard do.
        var pulsar = new[]
        {
            "..OOO...OOO..",
            ".............",
            "O....O.O....O",
            "O....O.O....O",
            "O....O.O....O",
            "..OOO...OOO..",
            ".............",
            "..OOO...OOO..",
            "O....O.O....O",
            "O....O.O....O",
            "O....O.O....O",
            ".............",
            "..OOO...OOO..",
        };

        var seed = PatternTestHelper.EmbedInGrid(pulsar, gridWidth: 33, gridHeight: 33, rowOffset: 10, colOffset: 10);
        var universe = CreateUniverse(seed);
        var fate = universe.DetermineFate(iterationBudget: 10, CancellationToken.None);

        var stabilized = Assert.IsType<Fate.Stabilized>(fate);
        Assert.Equal(0, stabilized.AtGeneration);
        Assert.Equal(3, stabilized.Period);
    }

    [Fact]
    public void Pentadecathlon_has_period_15()
    {
        // From LifeWiki's canonical RLE: x = 10, y = 3, 2bo4bo2b$2ob4ob2o$2bo4bo!
        // Transient generations spread a few cells beyond the seed's own 10x3 box, so it is padded.
        var pentadecathlon = new[]
        {
            "..O....O..",
            "OO.OOOO.OO",
            "..O....O..",
        };

        var seed = PatternTestHelper.EmbedInGrid(pentadecathlon, gridWidth: 30, gridHeight: 23, rowOffset: 10, colOffset: 10);
        var universe = CreateUniverse(seed);
        var fate = universe.DetermineFate(iterationBudget: 20, CancellationToken.None);

        var stabilized = Assert.IsType<Fate.Stabilized>(fate);
        Assert.Equal(0, stabilized.AtGeneration);
        Assert.Equal(15, stabilized.Period);
    }

    [Fact]
    public void Glider_translates_one_cell_diagonally_every_four_generations_while_clear_of_the_border()
    {
        var glider = new[]
        {
            ".O.",
            "..O",
            "OOO",
        };

        // Generously padded so the glider never approaches the bounded edge across 8 generations.
        var seed = PatternTestHelper.EmbedInGrid(glider, gridWidth: 20, gridHeight: 20, rowOffset: 2, colOffset: 2);
        var universe = CreateUniverse(seed);

        var generationFour = universe.GenerationAt(4, CancellationToken.None);
        var generationEight = universe.GenerationAt(8, CancellationToken.None);

        var expectedAtFour = PatternTestHelper.EmbedInGrid(glider, 20, 20, rowOffset: 3, colOffset: 3);
        var expectedAtEight = PatternTestHelper.EmbedInGrid(glider, 20, 20, rowOffset: 4, colOffset: 4);

        Assert.Equal(expectedAtFour, generationFour);
        Assert.Equal(expectedAtEight, generationEight);
        Assert.Equal(5, generationFour.Population);
    }

    [Fact]
    public void Diehard_dies_out_completely_at_exactly_generation_130()
    {
        var diehard = new[]
        {
            "......O.",
            "OO......",
            ".O...OOO",
        };

        // Diehard's flight excursion reaches roughly 20 cells away from the seed before it dies;
        // this margin keeps it clear of the bounded edge for all 130 generations.
        var seed = PatternTestHelper.EmbedInGrid(diehard, gridWidth: 50, gridHeight: 50, rowOffset: 15, colOffset: 15);
        var universe = CreateUniverse(seed);

        var generation129 = universe.GenerationAt(129, CancellationToken.None);
        var generation130 = universe.GenerationAt(130, CancellationToken.None);

        Assert.True(generation129.Population > 0);
        Assert.Equal(0, generation130.Population);
    }

    [Theory]
    [InlineData([new[] { ".OO", "OO.", ".O." }])] // R-pentomino
    [InlineData([new[] { ".O.....", "...O...", "OO..OOO" }])] // Acorn
    public void Methuselahs_eventually_converge_and_do_so_deterministically(string[] rows)
    {
        // These are used as convergence-and-determinism tests, not exact-generation-count tests: the
        // published stabilisation generations (1103 for R-pentomino, 5206 for Acorn) assume an
        // unbounded grid, and both patterns emit escaping gliders that die at a bounded border
        // instead of departing forever. See docs/design.md §6.4.
        var seed = PatternTestHelper.EmbedInGrid(rows, gridWidth: 60, gridHeight: 60, rowOffset: 25, colOffset: 25);
        var universe = CreateUniverse(seed);

        var firstFate = universe.DetermineFate(iterationBudget: 5000, CancellationToken.None);
        var secondFate = universe.DetermineFate(iterationBudget: 5000, CancellationToken.None);

        var stabilized = Assert.IsType<Fate.Stabilized>(firstFate);
        Assert.True(stabilized.AtGeneration >= 0);
        Assert.True(stabilized.Period >= 1);
        Assert.Equal(firstFate, secondFate);
    }

    [Fact]
    public void GenerationAt_is_a_pure_function_of_the_seed_rule_and_topology()
    {
        var seed = PatternTestHelper.FromAscii(
            "...",
            "OOO",
            "...");
        var universe = CreateUniverse(seed);

        var first = universe.GenerationAt(1, CancellationToken.None);
        var second = universe.GenerationAt(1, CancellationToken.None);

        Assert.Equal(first, second);
    }
}
