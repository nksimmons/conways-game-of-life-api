namespace GameOfLife.Domain.Domain;

/// <summary>Identity of a <see cref="Universe"/>. Minted by the caller before construction so the id is known before persistence.</summary>
public readonly record struct UniverseId(Guid Value)
{
    public static UniverseId NewId() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
