namespace GameOfLife.Domain.Domain;

/// <summary>A universe that wraps at the edges: the right edge joins the left, the top joins the bottom.</summary>
public sealed class ToroidalTopology : ITopology
{
    public static readonly ToroidalTopology Instance = new();

    private ToroidalTopology()
    {
    }

    public TopologyId Id => TopologyId.Toroidal;

    public int CountLiveNeighbors(Pattern pattern, int row, int col)
    {
        ArgumentNullException.ThrowIfNull(pattern);

        var count = 0;
        for (var deltaRow = -1; deltaRow <= 1; deltaRow++)
        {
            for (var deltaCol = -1; deltaCol <= 1; deltaCol++)
            {
                if (deltaRow == 0 && deltaCol == 0)
                {
                    continue;
                }

                var neighborRow = Wrap(row + deltaRow, pattern.Height);
                var neighborCol = Wrap(col + deltaCol, pattern.Width);
                if (pattern.IsAlive(neighborRow, neighborCol))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static int Wrap(int value, int modulus) => ((value % modulus) + modulus) % modulus;
}
