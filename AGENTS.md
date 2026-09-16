# AGENTS.md

Operating rules for AI agents working in this repository.

**Project:** RESTful API implementing Conway's Game of Life. **Target framework:** `net8.0`. Do not upgrade it; the exercise pins .NET 8. **Design document:** [docs/design.md](docs/design.md). Read it before making architectural changes. If a change contradicts it, update the document in the same change or do not make the change.

---

## 1. Architecture: Clean Architecture, enforced

### 1.1 The dependency rule

Dependencies point **inward only**.

```
Web  ->  UseCases  ->  Core  <-  Infrastructure
```

| Project | May reference | Must never reference |
|---|---|---|
| `Core` | **Nothing** | EF Core, ASP.NET Core, any I/O, any NuGet infrastructure package |
| `UseCases` | `Core` | `Infrastructure`, `Web`, EF Core, `HttpContext` |
| `Infrastructure` | `Core` | `Web`, `UseCases` |
| `Web` | `Core`, `UseCases`, `Infrastructure` | (nothing further) |

The single inward-violating reference (`Web -> Infrastructure`) exists **only** so the composition root can register implementations. No Web code may call an Infrastructure type directly; it may only name them inside DI registration.

### 1.2 This is verified, not trusted

Architecture tests assert the dependency rule. If you add a project reference that breaks it, the build fails. **Do not relax or delete these tests to make a change compile.** Fix the design instead.

### 1.3 Layer responsibilities

- **`Core`**: entities, value objects, domain services, rules, and ports (interfaces). Pure C# and BCL types only. Must remain unit-testable with no mocks, no fixtures, and no host.
- **`UseCases`**: command and query handlers that orchestrate domain objects and ports. Returns `Result<T>`. Contains no SQL, no HTTP, no serialization.
- **`Infrastructure`**: adapters implementing Core ports. The only project that knows a database exists.
- **`Web`**: controllers, serialization, middleware, composition root. Translates `Result<T>` into HTTP. Contains no business logic.

Controllers stay thin. A controller binds and validates the request, calls a handler, and maps the `Result<T>` onto a status code. If a controller grows a third responsibility, the logic belongs in a handler.

**There is no mediator library.** Dispatch is four interfaces in `UseCases`: `ICommand`, `IQuery<TResult>`, `ICommandHandler<TCommand>`, and `IQueryHandler<TQuery, TResult>`. Controllers inject the closed generics they need. Do not add MediatR or an equivalent to tidy up constructors; [docs/design.md §9.1](docs/design.md) records why, which alternatives were weighed, and what would justify reversing it.

Do not build a reflective `Send`. In particular, never use `Activator.CreateInstance` to resolve a handler: it bypasses dependency injection, defers missing-registration failures from startup to request time, and discards the generic constraint tying a query to its result type. Register handlers explicitly and enable `ServiceProviderOptions.ValidateOnBuild` so a missing registration fails the process at boot rather than on first request.

### 1.4 Ports, not wrappers

Interfaces in `Core` are **ports expressed in domain terms**.

```csharp
// CORRECT: domain vocabulary, no persistence concepts leak out
public interface IUniverseRepository
{
    Task<Universe?> FindAsync(UniverseId id, CancellationToken ct);
    Task AddAsync(Universe universe, CancellationToken ct);
}
```

Never do any of the following:

- Expose `IQueryable<T>` across the boundary.
- Accept `Expression<Func<T, bool>>` parameters.
- Define a generic `IRepository<T>` that forwards to `DbSet<T>`. **EF Core is already a repository and unit of work.** Wrapping it adds indirection and leaks the persistence model upward.
- Surface `SaveChanges` / `SaveChangesAsync` as a port method.

### 1.5 Domain purity

`Core` must have zero infrastructure dependencies. Specifically, no EF Core attributes, no `[JsonPropertyName]`, no `DbContext` awareness, no `IServiceProvider`. Persistence mapping belongs in Infrastructure (`IEntityTypeConfiguration<T>`); serialization shaping belongs in Web DTOs.

Domain objects validate their own invariants in constructors via guard clauses. An instance that exists is always valid.

---

## 2. Domain rules specific to this project

- **Use the Life literature's vocabulary, not the exercise's.** The aggregate is a `Universe`; its initial arrangement is the `Seed`; any arrangement of cells is a `Pattern`; the edge behaviour is `ITopology`; what a pattern eventually does is its `Fate`. "Board" is the exercise's word and appears only in HTTP routes, DTOs, and ProblemDetails. Do not introduce it into `Core` or `UseCases`, and do not invent friendlier synonyms ("experiment", "launch pattern", "boundary") for terms the literature already has. [docs/design.md §1.3](docs/design.md) is the reference.
- **Spaceship, glider, still life, and oscillator are observations, not fields.** They classify behaviour discovered by generating. Never add an `IsSpaceship` or `PatternKind` property to the model. They belong in tests and in response metadata derived from `Fate`, nowhere else.
- The **universe is an immutable seed.** Generations are computed, never stored. Do not add a "current generation" field or an `Advance()` method that mutates state. This single property underpins the caching, concurrency, and durability design, and breaking it invalidates all three.
- Generation *N* must be a **pure function** of the seed, rule, topology, and *N*. No clock, no randomness, no ambient state.
- Cell storage is **bit-packed** behind `Pattern`. Callers use `pattern.IsAlive(row, col)`. Never expose the backing array or make callers compute offsets.
- The evolution rule (`ILifeRule`) and topology (`ITopology`) are strategies. Do not hard-code B3/S23 or dead-edge logic into the aggregate.
- **Update generations simultaneously.** Every cell must read its neighbours' *previous* state. Updating in reading order produces "NaiveLife", a subtly different automaton and the most common bug in this problem. The Blinker and Glider tests exist to catch it; keep them passing.
- **There is no generation checkpointing, and `GET` handlers write nothing.** Checkpointing was designed and then cut, because bounding the input caps bounded worst-case cost directly ([docs/design.md §12](docs/design.md)). Do not reintroduce a snapshot table, a cache port, or a best-effort write on a read path.

  This is a decision taken on current numbers, not a permanent ban. At the present caps a generation costs roughly a tenth of a millisecond ([docs/design.md §10.1.1](docs/design.md)), so a cache would save little and cost a storage dependency, an eviction policy, and a write on a read path. ADR 010 records exactly what would justify reversing it: measured repeated deep reads of the same universe, an `ETag` hit ratio too low to absorb them, or a rise in the caps. Bring evidence from those metrics and reopen the ADR; do not add the mechanism back quietly.

---

## 3. C# and .NET practices

### 3.1 Asynchrony

- **Never `async void`.** The only exception is a true event handler, and there are none here.
- **Do not fake async.** The evolution core is CPU-bound and stays synchronous. Do not wrap pure computation in `Task.Run` to make it "async". That consumes a thread-pool thread without adding concurrency and starves the pool under load.
- Do not queue per-request work onto the thread pool. Never `Task.Run` in a request path.
- Thread `CancellationToken` through **every** async call and every long-running loop. In endpoints it originates from `HttpContext.RequestAborted`. A cancelled request must stop consuming CPU promptly.
- Never call `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()`.
- Use `ConfigureAwait(false)` in `Core`, `UseCases`, and `Infrastructure`. It is unnecessary in `Web`.
- Prefer `ValueTask` only where a hot path demonstrably allocates; default to `Task`.

### 3.2 Concurrency

There is no shared mutable state on the request path by design. Keep it that way. If synchronization ever becomes genuinely necessary, escalate in this order and justify each step in a comment:

1. **Immutability or a pure function.** Prefer eliminating the shared state.
2. **`ConcurrentDictionary`** (or another concurrent collection) when the data structure fits.
3. **`SemaphoreSlim`** for bounded concurrency and async-compatible waiting.
4. **`Mutex`** only if cross-process coordination is genuinely required.

Never take a coarse-grained lock around a request path. Never `lock` around an `await` (it will not compile with `await` inside, and the pattern indicates a design problem).

Bound concurrency explicitly, since unbounded parallelism is a defect. Use `SemaphoreSlim` admission gating sized from `Environment.ProcessorCount`, and return `503` with `Retry-After` when saturated rather than queueing without limit.

### 3.3 Dependency injection

- Default to **scoped** (per-request) or **transient** lifetimes.
- Only register a singleton when the type is genuinely stateless or immutable and thread-safe.
- Never capture a scoped service in a singleton (captive dependency).
- `HttpClient`: use `IHttpClientFactory`. Never `new HttpClient()` per request.
- Bind configuration with `IOptions<T>` and **validate on start**, so a bad configuration fails at boot rather than on first request.

### 3.4 Error handling

- Model **expected** failures as `Result<T>` values: not found, invalid input, non-convergence. `net8.0` has no discriminated unions; `Result<T>` is the substitute. Do not emulate union types with exceptions.
- Reserve exceptions for genuinely exceptional conditions.
- Never use exceptions for control flow.
- Never swallow an exception. If you catch, either handle meaningfully or rethrow with `throw;`, never `throw ex;`, which destroys the stack trace.
- All error responses are RFC 7807 `application/problem+json`. No stack traces, no internal paths, no SQL in any response body.

### 3.5 Validation and input safety

Validate at the **system boundary** (Web). Do not add defensive validation to internal methods for conditions that cannot occur.

Every one of these must be enforced and tested:

- Grid is non-empty and rectangular; reject ragged rows.
- Cell values are exactly `0` or `1`.
- Width and height are within `1..256`, and total cells within `65536`.
- `n` is non-negative and at most `1000`.
- The convergence budget defaults to `5000` iterations.
- Request body size is capped before parsing.

These caps are **denial-of-service controls**, not cosmetic validation, and they are the mechanism that bounds worst-case per-request cost. A request must never be able to demand unbounded CPU or memory.

Raising any of them is a design change, not a configuration tweak: it invalidates the arithmetic in [docs/design.md §10.1](docs/design.md) and may reopen the case for checkpointing. Do not treat admission control as a substitute either, since that bounds how many requests run at once rather than the cost of any one of them.

### 3.6 General

- Nullable reference types are enabled. Do not suppress with `!` to silence a warning; fix the nullability.
- Treat warnings as errors. Do not add `#pragma warning disable` to get past a build.
- Prefer `readonly record struct` / `record` for value objects.
- Prefer `sealed` classes by default.
- No `#region`. No commented-out code.
- Use `TimeProvider` rather than `DateTime.Now` / `DateTimeOffset.UtcNow` so time is testable. Prefer UTC everywhere.

---

## 4. API rules

- Versioned URL segment: `/api/v1/...`. Never add an unversioned route.
- Correct verbs: `POST` to create (server mints the id), `GET` for all projections. Every `GET` must be side-effect free and idempotent.
- `201 Created` includes a `Location` header.
- Generation responses carry `ETag` and `Cache-Control: public, immutable`, and honour `If-None-Match` with `304`. This is valid because generation content can never change.
- Status codes: `400` malformed/invalid input, `404` unknown board, `413` payload too large, `422` valid request that did not converge within budget, `503` admission rejected. Never return `500` for an anticipated condition.
- Hypermedia `_links` are generated from **route templates only**. Never issue a query to build a link.
- Every `GET` handler is side-effect free and writes nothing, including cache or checkpoint writes. HTTP allows some non-user-visible side effects on safe methods; this codebase deliberately does not use that latitude.
- Never log board contents, since payloads are large and add noise without adding diagnostic value. Log ids, dimensions, and outcomes.

---

## 5. Testing

Required before any change is considered complete:

- **Unit tests** for domain rules, using the published pattern oracles: Block (still life), Blinker/Toad (period 2), Pulsar (period 3), Penta-decathlon (period 15), Glider (translates (1,1) per 4 generations), Diehard (dies at 130), R-pentomino (stabilises at 1103).
- **Property tests** for determinism and purity.
- **Integration tests** against **real SQLite**, including a crash-simulation test: write, dispose the context, recreate, read back. Never use the EF Core `InMemory` provider to test persistence, since it cannot prove durability, and durability is an explicit requirement. The same suite must also run against Postgres via the compose profile; running it on two providers is what demonstrates the repository port actually holds, rather than merely asserting it.
- **Functional tests** through `WebApplicationFactory` covering status codes, ProblemDetails shape, `ETag`/`304`, and `Location`.
- **Architecture tests** asserting the dependency rule.

Test real behaviour. Do not assert that a mock was called as a substitute for asserting an outcome. Do not add methods to production types that exist only for tests. Do not weaken an assertion to make a failing test pass; diagnose the root cause first.

Pattern oracles from the published literature assume an **unbounded** grid. R-pentomino at 1103 generations and Acorn at 5206 both emit escaping gliders, which die at the border on a bounded grid, so those generation counts are not valid assertions here. Assert convergence and determinism for those two, not a specific generation. Contained patterns (Block, Blinker, Toad, Beacon, Pulsar, Penta-decathlon, Diehard) keep their exact published values.

---

## 6. Observability

Not optional, and not deferred to the end:

- Structured logging (Serilog, JSON to stdout) with a correlation id per request.
- OpenTelemetry traces with spans around evolution, checkpoint lookup, and database access.
- Metrics: generations computed, evolution duration histogram, checkpoint hit ratio, convergence outcomes, admission rejections.
- `/health/live` and `/health/ready` kept separate, so readiness checks the database and liveness checks only the process.

---

## 7. Scope discipline

This is a time-boxed exercise judged on **quality and thoughtful design, not completeness**. Over-engineering is a defect here, not a virtue.

- Make only changes that are requested or clearly necessary.
- Do not add features, layers, abstractions, or patterns beyond what is needed.
- Do not introduce Redis, Kafka, GraphQL, event sourcing, a job queue, worker nodes, or microservices. These are recorded in [docs/design.md §12](docs/design.md) as deliberately deferred, with rationale, and §8.3 of that document states the specific trigger that would justify a queue and a worker tier. If one becomes necessary, update those sections rather than silently adding it.
- Do not create abstractions for a single implementation unless the design document names it as an intentional extensibility seam. There are exactly three: `ILifeRule`, `ITopology`, and `IUniverseRepository`. A fourth needs a stated variation axis and a concrete second case, not a hypothetical one.
- There is exactly one bounded-context seam: between the exercise's HTTP contract ("board") and the Life domain. Do not split storage into its own "context"; the repository is a port within the Life context. A second context needs a term that means two different things on either side of it, and [docs/design.md §1.3](docs/design.md) names the one plausible candidate.
- Keep the architectural decision separate from the library implementing it. EF Core and Serilog are implementations; the decisions they serve are tabulated in [docs/design.md §9](docs/design.md). Swapping one of them is a small change. Changing what it is there for is not.
- Do not add documentation files unless asked.
- Do not add comments that restate what the code does. Comment only to explain something the code cannot show, such as a non-obvious constraint or rationale. One line is usually enough.

---

## 8. Writing prose in this repository

The design document is the most-read artifact here, so prose quality is part of the deliverable rather than incidental to it. These rules apply to Markdown, code comments, and commit messages.

- **No em dashes.** Use a colon, semicolon, comma, parentheses, or restructure the sentence.
- **Hedge contested judgments; assert verifiable facts.** "I tend to think bounded is the better default topology" is correct in tone. The pigeonhole argument about termination is not a matter of opinion and should be stated flatly. Do not hedge uniformly, and do not assert uniformly.
- **Paragraphs for reasoning, tables for enumerable facts.** If a table's cells contain arguments, it should be prose. Tables of endpoints, status codes, or configuration keys should stay tables.
- **No hype vocabulary.** Avoid "seamless", "robust", "cutting-edge", "effortless", and "leverage" as a verb.
- **Record corrections rather than overwriting them.** Where a decision reversed, say so and explain what changed the reasoning. That history is often the most useful part of a design document.
- Prefer specific over generic. Name the actual pattern, number, or failure mode.

---

## 9. Commands

```bash
dotnet build                      # warnings are errors
dotnet test                       # all suites
dotnet run --project src/*.Web    # SQLite file is created on first run
dotnet format --verify-no-changes # style gate
```

Never bypass safety gates (`--no-verify`, disabling analyzers, skipping tests) to land a change.
