using GameOfLife.Domain.Domain;
using GameOfLife.Domain.Rules;
using GameOfLife.UnitTests.TestSupport;

namespace GameOfLife.UnitTests.Domain;

public sealed class PatternTests
{
    [Fact]
    public void FromRows_reads_cells_in_row_major_order()
    {
        // Asymmetric under transposition on purpose. A diagonal seed would pass this test even if
        // rows and columns were swapped throughout, which is exactly what it exists to rule out.
        var pattern = PatternTestHelper.FromAscii(
            "OO",
            "..");

        Assert.Equal(2, pattern.Width);
        Assert.Equal(2, pattern.Height);
        Assert.Equal(2, pattern.Population);
        Assert.True(pattern.IsAlive(0, 0));
        Assert.True(pattern.IsAlive(0, 1));
        Assert.False(pattern.IsAlive(1, 0));
        Assert.False(pattern.IsAlive(1, 1));
    }

    [Fact]
    public void FromRows_rejects_empty_rows()
    {
        Assert.Throws<ArgumentException>(() => Pattern.FromRows([]));
    }

    [Fact]
    public void FromRows_rejects_ragged_rows()
    {
        var rows = new List<IReadOnlyList<int>>
        {
            new List<int> { 0, 1 },
            new List<int> { 0 }
        };

        Assert.Throws<ArgumentException>(() => Pattern.FromRows(rows));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void FromRows_rejects_a_null_row_with_an_argument_error(int nullRow)
    {
        var rows = new IReadOnlyList<int>[3];
        foreach (var row in Enumerable.Range(0, rows.Length))
            if (row != nullRow)
                rows[row] = new[] { 0, 1 };

        var error = Assert.Throws<ArgumentException>(() => Pattern.FromRows(rows));

        Assert.Equal("rows", error.ParamName);
    }

    [Fact]
    public void FromRows_rejects_values_other_than_zero_or_one()
    {
        var rows = new List<IReadOnlyList<int>>
        {
            new List<int> { 0, 2 }
        };

        Assert.Throws<ArgumentException>(() => Pattern.FromRows(rows));
    }

    [Fact]
    public void IsAlive_rejects_out_of_range_coordinates()
    {
        var pattern = PatternTestHelper.FromAscii("O");

        Assert.Throws<ArgumentOutOfRangeException>(() => pattern.IsAlive(1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => pattern.IsAlive(0, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => pattern.IsAlive(-1, 0));
    }

    [Fact]
    public void PackedBytes_round_trips_through_FromPackedBytes()
    {
        // Non-square, so that confusing width with height inside FromPackedBytes cannot survive:
        // both orderings pack into the same number of bytes, so only the dimensions expose a swap.
        var original = PatternTestHelper.FromAscii(
            "OO.",
            ".O.");

        var roundTripped = Pattern.FromPackedBytes(original.Width, original.Height, original.PackedBytes);

        Assert.Equal(original, roundTripped);
        Assert.Equal(original.Population, roundTripped.Population);
        Assert.Equal(3, roundTripped.Width);
        Assert.Equal(2, roundTripped.Height);
    }

    [Fact]
    public void Equals_compares_by_value_not_reference()
    {
        var a = PatternTestHelper.FromAscii("O.", ".O");
        var b = PatternTestHelper.FromAscii("O.", ".O");
        var c = PatternTestHelper.FromAscii("O.", ".O", "..");

        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
        Assert.NotEqual(a, c);
    }

    [Fact]
    public void ComputeHash_is_stable_and_sensitive_to_content()
    {
        var a = PatternTestHelper.FromAscii("O.", ".O");
        var b = PatternTestHelper.FromAscii("O.", ".O");
        var c = PatternTestHelper.FromAscii(".O", "O.");

        Assert.Equal(a.ComputeHash(), b.ComputeHash());
        Assert.NotEqual(a.ComputeHash(), c.ComputeHash());
    }

    [Fact]
    public void NextGeneration_updates_all_cells_simultaneously()
    {
        // A blinker read in reading order would have its middle cell's neighbour count corrupted by
        // the time the last cell is evaluated if updates were not simultaneous ("NaiveLife").
        var blinker = PatternTestHelper.FromAscii(
            "...",
            "OOO",
            "...");

        var next = blinker.NextGeneration(StandardLifeRule.Instance, BoundedTopology.Instance, CancellationToken.None);

        var expected = PatternTestHelper.FromAscii(
            ".O.",
            ".O.",
            ".O.");

        Assert.Equal(expected, next);
    }
}
