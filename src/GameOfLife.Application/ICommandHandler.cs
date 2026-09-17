using GameOfLife.Domain.Common;

namespace GameOfLife.Application;

public interface ICommandHandler<TCommand>
    where TCommand : ICommand
{
    Task<Result> HandleAsync(TCommand command, CancellationToken ct);
}
