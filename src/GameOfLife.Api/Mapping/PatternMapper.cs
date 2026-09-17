using GameOfLife.Domain.Domain;

namespace GameOfLife.Api.Mapping;

public static class PatternMapper
{
    public static int[][] ToRows(Pattern pattern)
    {
        var rows = new int[pattern.Height][];
        for (var row = 0; row < pattern.Height; row++)
        {
            var currentRow = new int[pattern.Width];
            for (var col = 0; col < pattern.Width; col++)
            {
                currentRow[col] = pattern.IsAlive(row, col) ? 1 : 0;
            }

            rows[row] = currentRow;
        }

        return rows;
    }

    /// <summary>A quoted ETag value. A generation's content can never change, so this is valid forever.</summary>
    public static string ComputeETag(Pattern pattern) => $"\"sha256-{pattern.ComputeHash().ToHexString()}\"";
}
