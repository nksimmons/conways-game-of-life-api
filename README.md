# conways-game-of-life-api

A RESTful API implementing Conway's Game of Life, built with .NET 8 and Clean Architecture.
See [docs/design.md](docs/design.md) for the design rationale and [AGENTS.md](AGENTS.md) for the
operating rules this codebase follows.

## Running

```bash
dotnet run --project src/GameOfLife.Api
```

A SQLite file (`gameoflife.db`) is created on first run in the working directory. Swagger UI is
available at `/swagger` in the Development environment.

## Testing

```bash
dotnet build ConwayGameOfLife.sln   # warnings are errors
dotnet test ConwayGameOfLife.sln    # unit, architecture, integration, and functional suites
dotnet format ConwayGameOfLife.sln --verify-no-changes
```

The integration suite runs against a real SQLite file by default. To also run it against Postgres:

```bash
docker compose --profile postgres up -d
GAMEOFLIFE_POSTGRES_CONNECTION_STRING="Host=localhost;Database=gameoflife;Username=gameoflife;Password=gameoflife" \
  dotnet test test/GameOfLife.IntegrationTests
```

Without that environment variable, the Postgres tests skip themselves rather than failing.
