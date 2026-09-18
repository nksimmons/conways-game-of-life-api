namespace GameOfLife.Application;

/// <summary>
/// Dispatch target for one <see cref="ICommand"/>. The generic constraint is load-bearing, not
/// decoration: it ties a command type to exactly one handler at compile time, which is what makes
/// registering handlers explicitly (rather than resolving them via <c>Activator.CreateInstance</c>
/// and reflection) both possible and safe. See AGENTS.md §1.3.
/// </summary>
public interface ICommandHandler<TCommand>
    where TCommand : ICommand
{
    Task HandleAsync(TCommand command, CancellationToken ct);
}
