namespace GameOfLife.IntegrationTests.TestSupport;

/// <summary>
/// A [Fact] that skips itself when GAMEOFLIFE_POSTGRES_CONNECTION_STRING is not set, i.e. when the
/// docker-compose "postgres" profile has not been started. Keeps the Postgres suite from failing CI
/// runs that have no database available while still exercising it wherever one is.
/// </summary>
public sealed class PostgresFactAttribute : FactAttribute
{
    public const string ConnectionStringEnvironmentVariable = "GAMEOFLIFE_POSTGRES_CONNECTION_STRING";

    public PostgresFactAttribute()
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(ConnectionStringEnvironmentVariable)))
        {
            Skip = $"Set {ConnectionStringEnvironmentVariable} to run against Postgres " +
                   "(docker compose --profile postgres up -d), e.g. " +
                   "\"Host=localhost;Database=gameoflife;Username=gameoflife;Password=gameoflife\".";
        }
    }
}
