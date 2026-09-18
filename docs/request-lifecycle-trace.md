# Request Lifecycle Trace

This guide follows a request through the running API, naming the actual objects
created along the way and the classes responsible for each hand-off. Start with
the four routes in `BoardsController`, then follow the links as needed. The
design vocabulary is deliberate: HTTP calls the resource a **board**, while the
domain calls the saved object a **universe**. The translation happens in the Api
layer and does not leak inward.

The fastest way to orient yourself is this:

```text
HTTP JSON
  -> Api contract and validation
  -> Application command or query handler
  -> Domain Universe and Pattern
  -> IUniverseRepository
  -> EF Core record and database
  -> Application view
  -> Api response JSON
```

The object that survives process restarts is the `Universe` seed, represented in
storage by `UniverseRecord`. A `Pattern` at generation N and a `Fate` are
computed when requested. They are never stored as a current state or checkpoint.

## 1. The cast of objects

| Layer | Object or class | Job in the lifecycle |
|---|---|---|
| Api | `UploadBoardRequest` | The incoming `{ cells: [[0, 1], ...] }` JSON body. It keeps the exercise's word, board. |
| Api | `BoardsController` | Binds and validates HTTP input, invokes exactly one use case, and turns the result into HTTP. |
| Api | `GenerationETagFilter` | Handles a conditional generation GET before the controller evolves a pattern. |
| Application | `CreateUniverseCommand` | Carries a new `UniverseId` and parsed seed into the create use case. |
| Application | `GetUniverseQuery`, `GetGenerationQuery`, `GetFinalStateQuery` | Carry the identity and the one read-specific input into their handlers. |
| Application | `CreateUniverseCommandHandler` | Builds and saves a `Universe` with the supported rule and topology. |
| Application | `GetUniverseQueryHandler` | Reads a universe and projects its saved facts into `UniverseView`. |
| Application | `GetGenerationQueryHandler` | Reads a universe and asks it for `GenerationAt(n)`. |
| Application | `GetFinalStateQueryHandler` | Reads a universe and asks it to `DetermineFate(budget)`. |
| Domain | `Universe` | Immutable aggregate. It owns the seed, rule, topology, and the two computations: generation and fate. |
| Domain | `Pattern` | Immutable value object for a single arrangement of cells. It bit-packs cells internally. |
| Domain | `ILifeRule` and `StandardLifeRule` | Decide whether one cell is alive next turn from its current state and live-neighbour count. |
| Domain | `ITopology` and `BoundedTopology` | Decide which cells count as neighbours at the edge of the universe. |
| Domain | `Fate` | Either a detected cycle (`Stabilized`) or an exhausted search budget (`Undetermined`). |
| Domain | `IUniverseRepository` | The domain-specific persistence port. It saves and finds universes, nothing generic. |
| Infrastructure | `EfUniverseRepository` | Converts between a domain `Universe` and a persistence `UniverseRecord`. |
| Infrastructure | `GameOfLifeDbContext` | EF Core's connection to the `Universes` table. |
| Api | `PatternMapper` | Converts a domain `Pattern` back into the row-major `int[][]` shape used by JSON. |

`Result<T>` is used only by query handlers for expected absence. `NotFound` means
the repository did not return a universe. It does not represent invalid input or
an undetermined fate.

## 2. What happens before a request arrives

[`Program.cs`](../src/GameOfLife.Api/Program.cs) builds the composition root.
This is where the live object graph is defined:

```csharp
builder.Services.AddScoped<ICommandHandler<CreateUniverseCommand>, CreateUniverseCommandHandler>();
builder.Services.AddScoped<IQueryHandler<GetGenerationQuery, PatternView>, GetGenerationQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetFinalStateQuery, FinalStateView>, GetFinalStateQueryHandler>();

builder.Services.AddInfrastructure(builder.Configuration);
```

`AddInfrastructure` registers `EfUniverseRepository` as the implementation of
`IUniverseRepository`, plus either SQLite or Postgres. Api is the only project
allowed to name Infrastructure, and it does so only through this registration.

At startup, the process also:

- validates `GameOfLifeOptions`, including request-size and generation limits;
- creates the database schema if it is absent;
- registers the `evaluation` concurrency policy used by generation and fate
  endpoints;
- wires structured logging, traces, metrics, health checks, exception handling,
  Swagger, and controllers;
- enables DI validation so an omitted handler registration fails at startup.

The rate limiter admits only a bounded number of CPU-heavy evaluations. It is
not involved in creating or retrieving a saved seed.

## 3. Flow A: POST a board and create a universe

### Example request

```http
POST /api/v1/boards
Content-Type: application/json

{
  "cells": [[0, 1, 0], [0, 0, 1], [1, 1, 1]]
}
```

### Step-by-step trace

1. ASP.NET Core binds the JSON body into
   [`UploadBoardRequest`](../src/GameOfLife.Api/Contracts/UploadBoardRequest.cs).
   Its `Cells` property is non-nullable, so a missing body or a missing `cells`
   member is rejected during binding.

2. [`BoardsController.CreateBoard`](../src/GameOfLife.Api/Controllers/BoardsController.cs)
   calls `UploadBoardRequestValidator` explicitly. The validator checks that the
   grid is non-empty and rectangular, has only `0` and `1` values, and stays
   inside the configured width, height, and total-cell limits. A malformed body
   or a validation failure becomes a consistent `400 application/problem+json`
   through the `ToProblemDetails()` extensions in `Errors`.

3. The controller makes two domain values:

   ```csharp
   var seed = Pattern.FromRows(board.Cells);
   var id = UniverseId.NewId();
   await createHandler.HandleAsync(new CreateUniverseCommand(id, seed), ct);
   ```

   `Pattern.FromRows` repeats the structural checks in the domain. That guard is
   not the HTTP error mechanism. It ensures a `Pattern` cannot be constructed
   invalidly by a future non-HTTP caller.

4. `CreateUniverseCommandHandler` creates the aggregate. Rule and topology are
   fixed here because the current HTTP contract does not offer a choice:

   ```csharp
   await repository.AddAsync(
       new Universe(
           command.UniverseId,
           command.Seed,
           RuleId.Standard,
           TopologyId.Bounded,
           timeProvider.GetUtcNow()),
       ct);
   ```

   The created `Universe` contains `Seed`, `Rule`, `Topology`, `Id`, and
   `CreatedAtUtc`. Its constructor resolves the rule and topology identifiers to
   the stateless `StandardLifeRule` and `BoundedTopology` strategy objects.

5. The `IUniverseRepository` reference is an `EfUniverseRepository` for the
   running service. It creates a `UniverseRecord`, copies the seed's packed bytes
   into a `byte[]`, adds the record to EF Core, and calls `SaveChangesAsync`.

6. Control returns to the controller. It creates `BoardCreatedResponse`, builds
   route-template links with `LinkGenerator`, and returns `201 Created` with the
   `Location` for `GetBoard`.

### The persisted shape

The database does not contain a `Universe` object. It contains this Infrastructure
record:

```csharp
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
```

That separation keeps EF Core attributes and tracking rules out of the Domain
project. The stored facts are enough to rebuild the same `Universe` after a
restart.

## 4. Flow B: GET the saved board

### Example request

```http
GET /api/v1/boards/{id}
```

1. The controller wraps the route `Guid` in `UniverseId` and constructs
   `GetUniverseQuery`.
2. `GetUniverseQueryHandler` calls `IUniverseRepository.FindAsync`.
3. `EfUniverseRepository` uses a point lookup with `AsNoTracking()`. Because a
   universe never changes after creation, there is no update for EF Core to
   track.
4. If a record exists, the repository rehydrates the domain aggregate:

   ```csharp
   var seed = Pattern.FromPackedBytes(record.Width, record.Height, record.SeedPacked);
   return new Universe(
       new UniverseId(record.Id),
       seed,
       new RuleId(record.RuleId),
       new TopologyId(record.TopologyId),
       record.CreatedAtUtc);
   ```

5. The handler creates a `UniverseView`. It is an Application response model,
   not a persistence record and not an HTTP DTO.
6. The controller projects `UniverseView` into `BoardResponse`. `PatternMapper`
   calls `Pattern.IsAlive(row, col)` for every cell to produce the JSON `cells`
   array. It does not expose the internal packed array.
7. If the repository returns `null`, the handler returns `Result.NotFound()` and
   the controller emits the board-shaped `404` problem response.

No Life evolution occurs in this flow.

## 5. Flow C: GET a numbered generation

### Example request

```http
GET /api/v1/boards/{id}/generations/4
```

This is the most useful trace to follow when learning how the domain works.

### C1. Admission and conditional-cache shortcut

The endpoint has `[EnableRateLimiting(EvaluationPolicy)]` and
`[GenerationETag]`.

`GenerationETagFilter` derives the validator from immutable identity and the
requested generation:

```csharp
var etag = new EntityTagHeaderValue($"\"v1-{id}-{targetGeneration}\"");
```

If `If-None-Match` contains that validator, its weak form (`W/"..."`), or `*`, the filter does a
cheap `GetUniverseQuery` only to verify that the board still exists. It then
returns `304 Not Modified`, adds the same ETag and a one-year immutable cache
policy, and never calls the generation handler. There is no pattern evolution in
this shortcut.

On a cache miss, the filter calls the controller and adds the cache headers only
when the controller returns `200 OK`.

### C2. Controller and application hand-off

`BoardsController.GetGeneration` first rejects an `n` below zero or above
`GameOfLifeOptions.MaxGenerationsAhead` with a `400`. Otherwise its private
`GenerationAsync` helper constructs:

```csharp
new GetGenerationQuery(new UniverseId(id), generation)
```

`GetGenerationQueryHandler` finds and rehydrates the universe through the
repository. It begins an `evolution.generate` activity, measures the operation,
and calls:

```csharp
var pattern = universe.GenerationAt(query.Generation, ct);
```

### C3. The domain loop

[`Universe.GenerationAt`](../src/GameOfLife.Domain/Domain/Universe.cs) starts
from the immutable `Seed` each time. Generation 4 is not based on a previous
request for generation 3:

```csharp
var current = Seed;
foreach (var _ in Enumerable.Range(0, n))
{
    ct.ThrowIfCancellationRequested();
    current = current.NextGeneration(_rule, _topology, ct);
}
return current;
```

Each iteration creates a new `Pattern`. The previous pattern is never modified.
That is what makes the update simultaneous.

Inside [`Pattern.NextGeneration`](../src/GameOfLife.Domain/Domain/Pattern.cs),
the inner loop asks two strategy objects one question each:

```csharp
var alive = IsAlive(row, col);
var liveNeighbors = topology.CountLiveNeighbors(this, row, col);

if (rule.NextState(alive, liveNeighbors))
{
    SetBit(nextBits, row * Width + col);
    population++;
}
```

`BoundedTopology.CountLiveNeighbors` ignores off-grid positions, treating them
as dead. `StandardLifeRule.NextState` implements B3/S23:

```csharp
(alive, liveNeighbors) switch
{
    (false, 3) => true,
    (true, 2 or 3) => true,
    _ => false
}
```

The new pattern is returned to the application handler as `PatternView`, then
to the controller as `GenerationResponse`. `PatternMapper.ToRows()` is the final
domain-to-JSON conversion.

The outer evolution loop uses `foreach` over row indices and a column array
reused across those rows. Neighbour counting iterates eight shared offsets.
`Pattern.FromRows` uses `rows.WithIndex()` and `currentRow.WithIndex()` to pair
each value with its position without asking callers to calculate storage offsets.

### C4. Cancellation and timing

The request's cancellation token travels from the controller into the handler,
repository, `Universe.GenerationAt`, and `Pattern.NextGeneration`. The domain
checks it once per generation and once per pattern row. A disconnected client
can therefore stop a long computation without waiting for all requested
generations to complete.

The generation handler records the generation count and elapsed evolution time
in `GameOfLifeDiagnostics`. These are observations of the request, not domain
state.

## 6. Flow D: GET the final state, meaning determine fate

### Example request

```http
GET /api/v1/boards/{id}/final
```

The route calls `GetFinalStateQueryHandler` with the configured
`FinalStateIterationBudget`. It rehydrates the universe exactly as the numbered
generation flow does, but it calls `Universe.DetermineFate` instead of
`GenerationAt`.

### D1. Cycle search

`DetermineFate` holds a dictionary from a pattern's SHA-256 `StateHash` to the
first generation at which the hash was seen:

```csharp
var seen = new Dictionary<StateHash, int>();
var current = Seed;
long generationsComputed = 0;

foreach (var generation in Enumerable.Range(0, Math.Max(1, iterationBudget)))
{
    ct.ThrowIfCancellationRequested();
    var hash = current.ComputeHash();
    if (seen.TryGetValue(hash, out var candidateGeneration))
    {
        var candidatePattern = GenerationAt(candidateGeneration, ct);
        generationsComputed += candidateGeneration;
        if (candidatePattern.Equals(current))
            return new Fate.Stabilized(candidateGeneration, generation - candidateGeneration,
                candidatePattern, generationsComputed);
    }

    seen[hash] = generation;
    if (generation + 1 >= iterationBudget) break;
    current = current.NextGeneration(_rule, _topology, ct);
    generationsComputed++;
}
return new Fate.Undetermined(iterationBudget, generationsComputed);
```

A hash match is only a candidate. The implementation recomputes the earlier
generation from the seed and checks structural `Pattern.Equals` before declaring
a cycle. This avoids treating an extremely unlikely hash collision as a real
fate.

`Fate.Stabilized(AtGeneration, Period, Pattern, GenerationsComputed)` covers both still lifes and oscillators:
a period of `1` is a still life; a larger period is an oscillator. If the budget
is reached first, the method returns `Fate.Undetermined(iterationBudget, generationsComputed)`. It
does not claim that the universe cannot eventually cycle.

### D2. Building the response

The handler turns the domain result into `FinalStateView`. On a detected cycle,
it uses the immutable pattern already computed and verified by the domain:

```csharp
Fate.Stabilized stabilized => new FinalStateView(
    universe.Id,
    stabilized.AtGeneration + stabilized.Period,
    new FinalStateView.Cycle(
        stabilized.AtGeneration,
        stabilized.Period,
        stabilized.Pattern))
```

The earlier implementation replayed the cycle start again here. That was
unnecessary: retaining the verified pattern for the response is neither a
checkpoint nor a write. For a cycle starting at s with period p, this removes
s evolution steps, reducing 3s + p to 2s + p. The generations-computed metric
uses `fate.GenerationsComputed`, including verification work; the response's
`IterationsExamined` remains search progress, s + p. The duration metric wraps
`DetermineFate`, which now contains all evolution work. For a single live cell,
the result is an empty still life at generation 1 with period 1: two search
steps plus one verification step, so the metric is 3, not 2.

The controller then has two honest exits:

- A `Cycle` becomes `FinalStateResponse` and `200 OK`.
- A null `Stabilized` value becomes `422 Unprocessable Entity`, with the number
  of generations examined in the problem detail.

The fate endpoint is rate-limited but deliberately not marked immutable. Its
answer depends on a server-side budget that can change.

## 7. Error and early-exit map

| Situation | First responsible component | HTTP result | Does it touch the database? | Does it evolve a pattern? |
|---|---|---:|---|---|
| Invalid JSON, missing body, or model-binding failure | ASP.NET Core plus `Errors` | 400 | No | No |
| Null row, ragged, non-binary, oversized, or empty upload | `UploadBoardRequestValidator` | 400 | No | No |
| Request body exceeds 1 MB | Kestrel plus `GlobalExceptionHandler` | 413 | No | No |
| Unknown board id on an ordinary GET | Query handler and controller | 404 | Yes, one lookup | No |
| Valid `If-None-Match` for an existing generation | `GenerationETagFilter` | 304 | Yes, one lookup | No |
| Generation index outside the configured limit | `BoardsController` | 400 | No | No |
| All evaluation permits are busy | Rate limiter in `Program` | 503 with `Retry-After` | No | No |
| Fate budget finishes without a cycle | `Universe.DetermineFate` and controller | 422 | Yes, one lookup | Yes, up to the budget |
| Unexpected exception | `GlobalExceptionHandler` | 500 problem response | Depends | Stops |
| Client disconnects | `CancellationToken` checks | No response is required | Any in-flight operation cancels | Stops at the next check |

## 8. Reading order in the debugger

For one full creation and retrieval trace, set breakpoints in this order:

1. `BoardsController.CreateBoard`
2. `UploadBoardRequestValidator`
3. `Pattern.FromRows`
4. `CreateUniverseCommandHandler.HandleAsync`
5. `EfUniverseRepository.AddAsync`
6. `GenerationETagFilter.OnActionExecutionAsync`
7. `BoardsController.GetGeneration` (only when the filter continues)
8. `GetGenerationQueryHandler.HandleAsync`
9. `EfUniverseRepository.FindAsync`
10. `Universe.GenerationAt`
11. `Pattern.NextGeneration`
12. `BoundedTopology.CountLiveNeighbors`
13. `StandardLifeRule.NextState`
14. `PatternMapper.ToRows`

For fate, replace steps 10 through 13 with `Universe.DetermineFate`,
`Pattern.ComputeHash`, and the `Fate.Stabilized` or `Fate.Undetermined` branch
in `GetFinalStateQueryHandler`.

When inspecting locals, watch these values:

| Point | Useful locals | What they tell you |
|---|---|---|
| Controller create path | `board`, `validationResult`, `seed`, `id` | Whether transport input became a valid domain seed. |
| Repository write | `universe`, `record` | The exact domain-to-storage translation. |
| Repository read | `record`, `seed`, returned `Universe` | The storage-to-domain reconstruction. |
| Generation loop | `current`, `i` | Which generation is being computed. |
| Cell loop | `alive`, `liveNeighbors`, `nextBits`, `population` | Why one cell lives, dies, or is born. |
| Fate loop | `generation`, `hash`, `candidateGeneration`, `seen` | Whether the search has found a possible repeat. |
| Final-state view | `fate`, `view.Stabilized`, `view.IterationsExamined` | Why the request becomes 200 or 422. |

## 9. Important invariants to keep in mind while tracing

- A `Universe` is immutable after construction. Do not look for an `Advance`
  method or a current-generation field.
- A `Pattern` is immutable. `NextGeneration` always returns a new instance.
- The Api project may use `Board` because it speaks the exercise contract. The
  Application and Domain projects use `Universe`, `Seed`, `Pattern`, `Topology`,
  and `Fate`.
- `IUniverseRepository` is the only persistence vocabulary exposed to the
  domain. EF Core appears only in Infrastructure.
- A successful generation GET writes nothing. A successful final-state GET also
  writes nothing.
- A conditional generation GET can return 304 without running the evolution
  loop, but it still checks that the universe exists.
- A `422` final-state result means the configured search budget ended. It is not
  proof that the pattern has no eventual cycle.
