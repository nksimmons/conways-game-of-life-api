namespace GameOfLife.Domain.Domain;

/// <summary>Identifies which <see cref="ILifeRule"/> a universe evolves under.</summary>
public readonly record struct RuleId(string Value)
{
    public static readonly RuleId Standard = new("B3/S23");

    public override string ToString() => Value;
}
