namespace GameOfLife.Infrastructure.Persistence;

/// <summary>
/// The persistence shape of a universe. Deliberately not the domain <c>Universe</c> type: EF Core
/// maps this plain record, and <see cref="EfUniverseRepository"/> translates between the two, so the
/// domain model never carries an EF Core attribute or constructor-binding constraint.
/// </summary>
internal sealed class UniverseRecord
{
    public Guid Id { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public string RuleId { get; set; } = string.Empty;

    public string TopologyId { get; set; } = string.Empty;

    public byte[] SeedPacked { get; set; } = Array.Empty<byte>();

    public DateTimeOffset CreatedAtUtc { get; set; }
}
