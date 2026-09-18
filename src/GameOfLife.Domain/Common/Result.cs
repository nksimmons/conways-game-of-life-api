namespace GameOfLife.Domain.Common;

public enum ResultStatus
{
    Success,
    NotFound,
}

/// <summary>
/// An expected failure modelled as a value rather than an exception. `net8.0` has no discriminated
/// unions; this is the substitute. Only the failures a handler can actually produce are represented:
/// invalid input is rejected at the web boundary before a handler runs, and non-convergence is a
/// successful answer carrying <see cref="GameOfLife.Domain.Domain.Fate"/>, not a failed one.
/// </summary>
public sealed class Result<T>
{
    private readonly T? _value;

    private Result(ResultStatus status, T? value)
    {
        Status = status;
        _value = value;
    }

    public ResultStatus Status { get; }

    public bool IsSuccess => Status == ResultStatus.Success;

    /// <summary>The value. Only meaningful when <see cref="IsSuccess"/>; accessing it otherwise is a programming error.</summary>
    public T Value => IsSuccess
        ? _value! // Success(T) is the only path that sets _value, and it takes a non-nullable T.
        : throw new InvalidOperationException($"Result has status {Status} and carries no value.");

    public static Result<T> Success(T value) => new(ResultStatus.Success, value);

    public static Result<T> NotFound() => new(ResultStatus.NotFound, default);
}
