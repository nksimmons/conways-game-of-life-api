using GameOfLife.Domain.Domain;
using GameOfLife.Infrastructure.Persistence;
using GameOfLife.IntegrationTests.TestSupport;
using Microsoft.EntityFrameworkCore;

namespace GameOfLife.IntegrationTests;

/// <summary>
/// Exercises <see cref="EfUniverseRepository"/> against a real SQLite file, never the EF Core
/// InMemory provider, because InMemory cannot prove durability and durability is the point.
/// </summary>
public sealed class SqliteUniverseRepositoryTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"gameoflife-test-{Guid.NewGuid():N}.db");

    private GameOfLifeDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<GameOfLifeDbContext>()
            .UseSqlite($"Data Source={_dbPath}")
            .Options;
        var context = new GameOfLifeDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task AddAsync_then_FindAsync_round_trips_the_universe()
    {
        using var context = CreateContext();
        var repository = new EfUniverseRepository(context);
        var universe = SampleUniverseFactory.Create();

        await repository.AddAsync(universe, CancellationToken.None);
        var found = await repository.FindAsync(universe.Id, CancellationToken.None);

        Assert.NotNull(found);
        Assert.Equal(universe.Id, found!.Id);
        Assert.Equal(universe.Seed, found.Seed);
        Assert.Equal(universe.Rule, found.Rule);
        Assert.Equal(universe.Topology, found.Topology);
    }

    [Fact]
    public async Task FindAsync_returns_null_for_an_unknown_id()
    {
        using var context = CreateContext();
        var repository = new EfUniverseRepository(context);

        var found = await repository.FindAsync(UniverseId.NewId(), CancellationToken.None);

        Assert.Null(found);
    }

    [Fact]
    public async Task Universe_survives_write_dispose_recreate_read_back()
    {
        var universe = SampleUniverseFactory.Create();

        // Write and fully dispose the context (and its connection), simulating a process crash/restart.
        using (var writeContext = CreateContext())
        {
            var writeRepository = new EfUniverseRepository(writeContext);
            await writeRepository.AddAsync(universe, CancellationToken.None);
        }

        // Open an entirely new context against the same file, as a freshly started process would.
        var readOptions = new DbContextOptionsBuilder<GameOfLifeDbContext>()
            .UseSqlite($"Data Source={_dbPath}")
            .Options;
        using var readContext = new GameOfLifeDbContext(readOptions);
        var readRepository = new EfUniverseRepository(readContext);

        var found = await readRepository.FindAsync(universe.Id, CancellationToken.None);

        Assert.NotNull(found);
        Assert.Equal(universe.Seed, found!.Seed);
        Assert.Equal(universe.Rule, found.Rule);
        Assert.Equal(universe.Topology, found.Topology);
    }

    public void Dispose()
    {
        if (File.Exists(_dbPath))
        {
            File.Delete(_dbPath);
        }
    }
}
