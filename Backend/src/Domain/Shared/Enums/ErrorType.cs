namespace Domain.Shared.Errors;

public enum ErrorType
{
    None = 1,
    Validation = 400,
    BadRequest = Validation,
    NotFound = 404,
    Unauthorized = 401,
    Conflict = 409,
    Failure = 500,
    InternalServerError = Failure
}
