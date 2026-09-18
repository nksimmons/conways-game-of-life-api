using GameOfLife.Domain.Domain;

namespace GameOfLife.Application.CreateUniverse;

public sealed class CreateUniverseCommandHandler(IUniverseRepository repository, TimeProvider timeProvider)
    : ICommandHandler<CreateUniverseCommand>
{
    public async Task HandleAsync(CreateUniverseCommand command, CancellationToken ct)
    {
        // Rule and topology are hardcoded rather than accepted from the request: the API exposes only
        // the exercise's board upload, with no field for choosing either. Both stay behind ILifeRule
        // and ITopology so a second one is a new implementation, not a rewrite; see docs/design.md §4.3.
        var universe = new Universe(
            command.UniverseId,
            command.Seed,
            RuleId.Standard,
            TopologyId.Bounded,
            timeProvider.GetUtcNow());

        await repository.AddAsync(universe, ct).ConfigureAwait(false);
    }
}
