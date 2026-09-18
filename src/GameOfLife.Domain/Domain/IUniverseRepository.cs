namespace GameOfLife.Domain.Domain;

/// <summary>The Life context's port for saving and finding universes. Stores exactly one kind of thing.</summary>
public interface IUniverseRepository
{
    Task<Universe?> FindAsync(UniverseId id, CancellationToken ct);

    Task AddAsync(Universe universe, CancellationToken ct);
}