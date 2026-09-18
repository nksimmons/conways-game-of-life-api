namespace GameOfLife.Domain.Common;

/// <summary>Extension methods for enumerable collections.</summary>
public static class EnumerableExtensions
{
    /// <summary>Projects each element into a tuple pairing the item with its zero-based index.</summary>
    public static IEnumerable<(T Item, int Index)> WithIndex<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return source.Select((item, index) => (item, index));
    }
}
