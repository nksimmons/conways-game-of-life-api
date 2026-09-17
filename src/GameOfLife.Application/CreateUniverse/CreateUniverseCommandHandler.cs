using GameOfLife.Domain.Common;
using GameOfLife.Domain.Domain;

namespace GameOfLife.Application.CreateUniverse;

public sealed class CreateUniverseCommandHandler : ICommandHandler<CreateUniverseCommand>
{
    private readonly IUniverseRepository _repository;
    private readonly TimeProvider _timeProvider;

    public CreateUniverseCommandHandler(IUniverseRepository repository, TimeProvider timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public async Task<Result> HandleAsync(CreateUniverseCommand command, CancellationToken ct)
    {
        var universe = new Universe(
            command.UniverseId,
            command.Seed,
            RuleId.Standard,
            TopologyId.Bounded,
            _timeProvider.GetUtcNow());

        await _repository.AddAsync(universe, ct).ConfigureAwait(false);
        return Result.Success();
    }
}
