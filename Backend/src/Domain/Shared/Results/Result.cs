using Domain.Shared.Errors;

namespace Domain.Shared.Results;

public class Result
{
    public bool IsSuccess { get; private set; }
    public Error Error { get; private set; }
    public string? ErrorMessage => IsSuccess ? null : Error.Description;
    public ErrorType ErrorType => Error.Type;

    protected Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success => new(true, Error.None);

    public static Result Failure(Error error)
    {
        return new Result(false, error);
    }

    public static Result Failure(Error error, Exception ex)
    {
        return Failure(Error.FromType(error.Type, $"{error.Description}: {ex.Message}", error.Code));
    }

    public static Result Failure(string errorMessage, ErrorType errorType = ErrorType.Failure)
    {
        return Failure(Error.FromType(errorType, errorMessage));
    }

    public static Result NotFound(string entity)
    {
        return Failure(Error.NotFound($"{entity} not found"));
    }

    public static Result InvalidId()
    {
        return Failure(Error.InvalidId());
    }

    public static Result Exception(string methodName, Exception ex)
    {
        return Failure(Error.Exception(methodName, ex));
    }

    public static Result Exception(string methodName, string className, Exception ex)
    {
        return Failure(Error.Exception(methodName, className, ex));
    }
}
