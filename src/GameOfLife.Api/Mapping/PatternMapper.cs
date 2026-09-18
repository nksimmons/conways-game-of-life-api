using GameOfLife.Domain.Domain;

namespace GameOfLife.Api.Mapping;

/// <summary>
///     Projects domain patterns into transport representations. Domain patterns use bit-packed
///     backing storage to bound memory, while HTTP responses expose row-major integer grids for JSON.
/// </summary>
public static class PatternMapper
{
    /// <summary>
    ///     Extracts a 2D row-major array of 0/1 cell values by querying <see cref="Pattern.IsAlive" />,
    ///     preserving the domain object's internal packing encapsulation.
    /// </summary>
    public static int[][] ToRows(this Pattern pattern)
    {
        ArgumentNullException.ThrowIfNull(pattern);

        return Enumerable.Range(0, pattern.Height)
            .Select(row => Enumerable.Range(0, pattern.Width)
                .Select(col => pattern.IsAlive(row, col) ? 1 : 0)
                .ToArray())
            .ToArray();
    }
}
