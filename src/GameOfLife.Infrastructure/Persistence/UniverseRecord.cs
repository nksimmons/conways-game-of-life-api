namespace GameOfLife.Infrastructure.Persistence;

/// <summary>
///     The persistence shape of a universe. Deliberately not the domain <c>Universe</c> type: EF Core
///     maps this plain record, and <see cref="EfUniverseRepository" /> translates between the two, so the
///     domain model never carries an EF Core attribute or constructor-binding constraint.
/// </summary>
internal sealed class UniverseRecord
{
    public Guid Id { get; init; }

    public int Width { get; init; }

    public int Height { get; init; }

    public string RuleId { get; init; } = string.Empty;

    public string TopologyId { get; init; } = string.Empty;

    public byte[] SeedPacked { get; init; } = [];

    public DateTimeOffset CreatedAtUtc { get; init; }
}