using GameOfLife.Domain.Domain;
using GameOfLife.Infrastructure.HealthChecks;
using GameOfLife.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameOfLife.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    private const string SqliteDefaultConnectionString = "Data Source=gameoflife.db";

    /// <summary>
    /// Registers the EF Core adapter for <see cref="IUniverseRepository"/>. Provider is chosen by
    /// configuration (<c>Persistence:Provider</c> = <c>Sqlite</c>, the default, or <c>Postgres</c>);
    /// see docs/design.md §7.3 for why SQLite is the default and what would change it.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["Persistence:Provider"] ?? "Sqlite";

        services.AddDbContext<GameOfLifeDbContext>(options =>
        {
            switch (provider)
            {
                case "Postgres":
                    var postgresConnectionString = configuration.GetConnectionString("GameOfLife")
                        ?? throw new InvalidOperationException(
                            "Connection string 'GameOfLife' is required when Persistence:Provider is 'Postgres'.");
                    options.UseNpgsql(postgresConnectionString);
                    break;
                case "Sqlite":
                    options.UseSqlite(configuration.GetConnectionString("GameOfLife") ?? SqliteDefaultConnectionString);
                    break;
                default:
                    throw new InvalidOperationException(
                        $"Unknown Persistence:Provider '{provider}'. Expected 'Sqlite' or 'Postgres'.");
            }
        });

        services.AddScoped<IUniverseRepository, EfUniverseRepository>();

        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database", tags: new[] { "ready" });

        return services;
    }

    /// <summary>
    /// Creates the schema if it does not already exist. Called once at startup instead of using EF
    /// Migrations, since this is a single insert-only table with no schema evolution to manage. The
    /// only Web-layer call site is `app.Services.EnsureDatabaseCreated()`; Web never references
    /// <see cref="GameOfLifeDbContext"/> directly.
    /// </summary>
    public static void EnsureDatabaseCreated(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameOfLifeDbContext>();
        dbContext.Database.EnsureCreated();
    }
}
