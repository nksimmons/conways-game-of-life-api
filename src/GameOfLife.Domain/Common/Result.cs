namespace GameOfLife.Domain.Common;

public enum ResultStatus
{
    Success,
    NotFound,
    Invalid,
}

/// <summary>A non-generic expected-failure result. `net8.0` has no discriminated unions; this is the substitute.</summary>
public class Result
{
    public ResultStatus Status { get; }

    public IReadOnlyList<string> Errors { get; }

    public bool IsSuccess => Status == ResultStatus.Success;

    protected Result(ResultStatus status, IReadOnlyList<string>? errors = null)
    {
        Status = status;
        Errors = errors ?? Array.Empty<string>();
    }

    public static Result Success() => new(ResultStatus.Success);

    public static Result NotFound() => new(ResultStatus.NotFound);

    public static Result Invalid(params string[] errors) => new(ResultStatus.Invalid, errors);
}

/// <summary>A <see cref="Result"/> that carries a value on success.</summary>
public sealed class Result<T> : Result
{
    private readonly T? _value;

    /// <summary>The value. Only meaningful when <see cref="Result.IsSuccess"/>; accessing it otherwise is a programming error.</summary>
    public T Value => IsSuccess
        ? _value! // Success(T) is the only path that sets _value, and it takes a non-nullable T.
        : throw new InvalidOperationException($"Result has status {Status} and carries no value.");

    private Result(ResultStatus status, T? value, IReadOnlyList<string>? errors = null)
        : base(status, errors)
    {
        _value = value;
    }

    public static Result<T> Success(T value) => new(ResultStatus.Success, value);

    public static new Result<T> NotFound() => new(ResultStatus.NotFound, default);

    public static new Result<T> Invalid(params string[] errors) => new(ResultStatus.Invalid, default, errors);
}
