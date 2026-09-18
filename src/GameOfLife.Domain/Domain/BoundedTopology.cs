namespace GameOfLife.Domain.Domain;

/// <summary>A finite universe with a dead edge: cells beyond the boundary are always dead. The default topology.</summary>
public sealed class BoundedTopology : ITopology
{
    // Stateless strategy: a singleton avoids reallocating per neighbour count, and it is safe to
    // share because CountLiveNeighbors never mutates instance state. See AGENTS.md §3.3.
    public static readonly BoundedTopology Instance = new();

    private BoundedTopology()
    {
    }

    public TopologyId Id => TopologyId.Bounded;

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

                var neighborRow = row + deltaRow;
                var neighborCol = col + deltaCol;
                if (neighborRow < 0 || neighborRow >= pattern.Height || neighborCol < 0 || neighborCol >= pattern.Width)
                {
                    continue;
                }

                if (pattern.IsAlive(neighborRow, neighborCol))
                {
                    count++;
                }
            }
        }

        return count;
    }
}
