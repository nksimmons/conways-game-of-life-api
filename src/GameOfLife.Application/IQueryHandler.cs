using GameOfLife.Domain.Common;

namespace GameOfLife.Application;

/// <summary>
/// Dispatch target for one <see cref="IQuery{TResult}"/>. See <see cref="ICommandHandler{TCommand}"/>
/// for why the generic constraint, and not a reflective <c>Send</c>, is the point.
/// </summary>
public interface IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<Result<TResult>> HandleAsync(TQuery query, CancellationToken ct);
}
