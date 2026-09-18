namespace GameOfLife.Domain.Domain;

/// <summary>Resolves the well-known topologies by <see cref="TopologyId" />.</summary>
public static class Topologies
{
    private static readonly (int Row, int Col)[] Offsets =
        [(-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1)];

    internal static ReadOnlySpan<(int Row, int Col)> NeighborOffsets => Offsets;

    public static ITopology Resolve(this TopologyId id)
    {
        if (id == TopologyId.Bounded) return BoundedTopology.Instance;

        if (id == TopologyId.Toroidal) return ToroidalTopology.Instance;

        throw new InvalidOperationException($"Unknown topology '{id}'.");
    }
}
