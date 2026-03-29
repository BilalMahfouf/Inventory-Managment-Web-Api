using Domain.Shared.Errors;

namespace Domain.Shared.Results;

public class Result<T> : Result
{
    private T? _value;
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    private Result(bool isSuccess, T? value, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public static new Result<T> Success(T value)
    {
        return new Result<T>(true, value, Error.None);
    }

    public static new Result<T> Failure(Error error)
    {
        return new Result<T>(false, default, error);
    }

    public static new Result<T> Failure(Error error, Exception ex)
    {
        return Failure(Error.FromType(error.Type, $"{error.Description}: {ex.Message}", error.Code));
    }

    public static new Result<T> Failure(string error, ErrorType errorType = ErrorType.Failure)
    {
        return Failure(Error.FromType(errorType, error));
    }

    public static new Result<T> NotFound(string entity)
    {
        return Failure(Error.NotFound($"{entity} not found"));
    }

    public static new Result<T> InvalidId()
    {
        return Failure(Error.InvalidId());
    }

    public static new Result<T> Exception(string methodName, Exception ex)
    {
        return Failure(Error.Exception(methodName, ex));
    }

    public static new Result<T> Exception(string methodName, string className, Exception ex)
    {
        return Failure(Error.Exception(methodName, className, ex));
    }
}
