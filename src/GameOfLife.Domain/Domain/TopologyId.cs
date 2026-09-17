namespace GameOfLife.Domain.Domain;

/// <summary>Identifies which <see cref="ITopology"/> a universe's edge behaviour follows.</summary>
public readonly record struct TopologyId(string Value)
{
    public static readonly TopologyId Bounded = new("bounded");
    public static readonly TopologyId Toroidal = new("toroidal");

    public override string ToString() => Value;
}
