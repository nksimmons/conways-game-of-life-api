using GameOfLife.Domain.Domain;

namespace GameOfLife.UnitTests.TestSupport;

/// <summary>Builds a <see cref="Pattern"/> from an ASCII grid: 'O' is alive, anything else is dead.</summary>
internal static class PatternTestHelper
{
    public static Pattern FromAscii(params string[] rows)
    {
        var grid = rows
            .Select(row => (IReadOnlyList<int>)[.. row.Select(ch => ch == 'O' ? 1 : 0)])
            .ToList();

        return Pattern.FromRows(grid);
    }

    /// <summary>Embeds a small ASCII pattern into the top-left of a larger dead grid, so a spaceship or
    /// methuselah has room to move without hitting the bounded edge during the generations under test.</summary>
    public static Pattern EmbedInGrid(string[] rows, int gridWidth, int gridHeight, int rowOffset, int colOffset)
    {
        var grid = new int[gridHeight][];
        for (var r = 0; r < gridHeight; r++)
        {
            grid[r] = new int[gridWidth];
        }

        for (var r = 0; r < rows.Length; r++)
        {
            for (var c = 0; c < rows[r].Length; c++)
            {
                if (rows[r][c] == 'O')
                {
                    grid[r + rowOffset][c + colOffset] = 1;
                }
            }
        }

        return Pattern.FromRows([.. grid.Select(row => (IReadOnlyList<int>)row)]);
    }
}
