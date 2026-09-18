using GameOfLife.Domain.Common;

namespace GameOfLife.UnitTests.Common;

public sealed class EnumerableExtensionsTests
{
    [Fact]
    public void WithIndex_pairs_values_with_zero_based_positions()
    {
        var fruits = new[] { "Apple", "Banana", "Cherry" };

        Assert.Equal([("Apple", 0), ("Banana", 1), ("Cherry", 2)], fruits.WithIndex());
    }

    [Fact]
    public void WithIndex_preserves_empty_sequences() => Assert.Empty(Array.Empty<int>().WithIndex());

    [Fact]
    public void WithIndex_defers_enumeration_and_restarts_indices_for_each_enumeration()
    {
        var values = new List<int> { 7 };
        var indexed = values.WithIndex();
        values.Add(9);

        Assert.Equal([(7, 0), (9, 1)], indexed);
        Assert.Equal([(7, 0), (9, 1)], indexed);
    }
}
