namespace CloudMusicPlayer.Core.Errors;

public readonly record struct Result
{
    public Result()
    {
        IsSuccess = true;
    }

    private Result(Error error)
    {
        IsSuccess = false;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success() => new Result();
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    public static Result Failure(Error error) => new Result(error);
    public static Result<T> Failure<T>(Error error) => Result<T>.Failure(error);
}

public readonly struct Result<T>
{
    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    private Result(Error error)
    {
        IsSuccess = false;
        Error = error;
    }

    public bool IsSuccess { get; }
    public T Value
    {
        get;
    }

    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result<T> Success(T value) => new Result<T>(value);
    public static Result<T> Failure(Error error) => new Result<T>(error);
}

