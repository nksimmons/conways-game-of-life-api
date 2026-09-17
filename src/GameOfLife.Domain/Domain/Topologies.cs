namespace GameOfLife.Domain.Domain;

/// <summary>Resolves the well-known topologies by <see cref="TopologyId"/>.</summary>
public static class Topologies
{
    public static ITopology Resolve(TopologyId id)
    {
        if (id == TopologyId.Bounded)
        {
            return BoundedTopology.Instance;
        }

        if (id == TopologyId.Toroidal)
        {
            return ToroidalTopology.Instance;
        }

        throw new InvalidOperationException($"Unknown topology '{id}'.");
    }
}
