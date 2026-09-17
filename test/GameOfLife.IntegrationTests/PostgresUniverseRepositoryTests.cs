using GameOfLife.Domain.Domain;
using GameOfLife.Infrastructure.Persistence;
using GameOfLife.IntegrationTests.TestSupport;
using Microsoft.EntityFrameworkCore;

namespace GameOfLife.IntegrationTests;

/// <summary>
/// The same repository contract as <see cref="SqliteUniverseRepositoryTests"/>, run against Postgres.
/// Running the identical suite against a second engine demonstrates that the <c>IUniverseRepository</c>
/// port actually holds, rather than merely asserting that it would. Requires the docker-compose
/// "postgres" profile; see <see cref="PostgresFactAttribute"/>.
/// </summary>
public sealed class PostgresUniverseRepositoryTests : IAsyncLifetime
{
    private readonly string _connectionString =
        Environment.GetEnvironmentVariable(PostgresFactAttribute.ConnectionStringEnvironmentVariable) ?? string.Empty;

    private GameOfLifeDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<GameOfLifeDbContext>()
            .UseNpgsql(_connectionString)
            .Options;
        return new GameOfLifeDbContext(options);
    }

    public async Task InitializeAsync()
    {
        if (string.IsNullOrEmpty(_connectionString))
        {
            return;
        }

        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [PostgresFact]
    public async Task AddAsync_then_FindAsync_round_trips_the_universe()
    {
        await using var context = CreateContext();
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

    [PostgresFact]
    public async Task FindAsync_returns_null_for_an_unknown_id()
    {
        await using var context = CreateContext();
        var repository = new EfUniverseRepository(context);

        var found = await repository.FindAsync(UniverseId.NewId(), CancellationToken.None);

        Assert.Null(found);
    }

    [PostgresFact]
    public async Task Universe_survives_write_dispose_recreate_read_back()
    {
        var universe = SampleUniverseFactory.Create();

        await using (var writeContext = CreateContext())
        {
            var writeRepository = new EfUniverseRepository(writeContext);
            await writeRepository.AddAsync(universe, CancellationToken.None);
        }

        await using var readContext = CreateContext();
        var readRepository = new EfUniverseRepository(readContext);

        var found = await readRepository.FindAsync(universe.Id, CancellationToken.None);

        Assert.NotNull(found);
        Assert.Equal(universe.Seed, found!.Seed);
        Assert.Equal(universe.Rule, found.Rule);
        Assert.Equal(universe.Topology, found.Topology);
    }
}
