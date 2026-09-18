namespace GameOfLife.Domain.Domain;

/// <summary>The universe's edge behaviour: what a cell at the boundary counts as its off-grid neighbours.</summary>
public interface ITopology
{
    TopologyId Id { get; }

    int CountLiveNeighbors(Pattern pattern, int row, int col);
}