namespace GameOfLife.Domain.Domain;

/// <summary>A universe that wraps at the edges: the right edge joins the left, the top joins the bottom.</summary>
public sealed class ToroidalTopology : ITopology
{
    // See BoundedTopology.Instance for why this is a singleton.
    public static readonly ToroidalTopology Instance = new();

    private ToroidalTopology()
    {
    }

    public TopologyId Id => TopologyId.Toroidal;

    public int CountLiveNeighbors(Pattern pattern, int row, int col)
    {
        ArgumentNullException.ThrowIfNull(pattern);

        var count = 0;
        foreach (var (deltaRow, deltaCol) in Topologies.NeighborOffsets)
        {
            var neighborRow = Wrap(row + deltaRow, pattern.Height);
            var neighborCol = Wrap(col + deltaCol, pattern.Width);
            if (pattern.IsAlive(neighborRow, neighborCol)) count++;
        }

        return count;
    }

    private static int Wrap(int value, int modulus)
    {
        // C#'s % can return a negative result for a negative dividend (e.g. -1 % 32 == -1, not 31),
        // so a single mod is not enough to wrap row/col - 1 at index 0. The extra +modulus, %modulus
        // shifts the result back into [0, modulus) regardless of the sign of the input.
        return (value % modulus + modulus) % modulus;
    }
}
