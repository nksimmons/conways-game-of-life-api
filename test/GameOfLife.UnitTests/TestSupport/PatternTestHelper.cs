using GameOfLife.Domain.Common;
using GameOfLife.Domain.Domain;

namespace GameOfLife.UnitTests.TestSupport;

/// <summary>Builds a <see cref="Pattern" /> from an ASCII grid: 'O' is alive, anything else is dead.</summary>
internal static class PatternTestHelper
{
    public static Pattern FromAscii(params string[] rows) =>
        Pattern.FromRows(rows
            .Select(row => (IReadOnlyList<int>)[.. row.Select(ch => ch == 'O' ? 1 : 0)])
            .ToList());

    /// <summary>
    ///     Embeds a small ASCII pattern into the top-left of a larger dead grid, so a spaceship or
    ///     methuselah has room to move without hitting the bounded edge during the generations under test.
    /// </summary>
    public static Pattern EmbedInGrid(string[] rows, int gridWidth, int gridHeight, int rowOffset, int colOffset)
    {
        var grid = Enumerable.Range(0, gridHeight).Select(_ => new int[gridWidth]).ToArray();

        foreach (var (row, r) in rows.WithIndex())
            foreach (var (cell, c) in row.WithIndex())
                if (cell == 'O')
                    grid[r + rowOffset][c + colOffset] = 1;

        return Pattern.FromRows([.. grid.Select(row => (IReadOnlyList<int>)row)]);
    }
}
