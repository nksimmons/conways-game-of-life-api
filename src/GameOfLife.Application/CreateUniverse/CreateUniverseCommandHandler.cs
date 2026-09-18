using GameOfLife.Domain.Domain;
using GameOfLife.Domain.Rules;

namespace GameOfLife.Application.CreateUniverse;

/// <summary>Handles board creation by persisting a new universe aggregate.</summary>
public sealed class CreateUniverseCommandHandler(IUniverseRepository repository, TimeProvider timeProvider)
    : ICommandHandler<CreateUniverseCommand>
{
    public async Task HandleAsync(CreateUniverseCommand command, CancellationToken ct) =>
        await repository.AddAsync(
            new Universe(
                command.UniverseId,
                command.Seed,
                command.Rule ?? RuleId.Standard,
                command.Topology ?? TopologyId.Bounded,
                timeProvider.GetUtcNow()), ct);
}