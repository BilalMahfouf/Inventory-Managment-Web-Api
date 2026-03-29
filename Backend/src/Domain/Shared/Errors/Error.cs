namespace Domain.Shared.Errors;

public sealed class Error
{
    public string Code { get; private set; }
    public string Description { get; private set; }
    public ErrorType Type { get; private set; }

    private Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    public static Error None => new("Error.None", "No error.", ErrorType.None);

    public static Error Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);

    public static Error Failure(string description) =>
        new("Error.Failure", description, ErrorType.Failure);

    public static Error Validation(string code, string description) =>
        new(code, description, ErrorType.Validation);

    public static Error Validation(string description) =>
        new("Error.Validation", description, ErrorType.Validation);

    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    public static Error NotFound(string description) =>
        new("Error.NotFound", description, ErrorType.NotFound);

    public static Error NotFound(object entity) =>
        new("Error.NotFound", $"{entity} not found", ErrorType.NotFound);

    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);

    public static Error Conflict(string description) =>
        new("Error.Conflict", description, ErrorType.Conflict);

    public static Error Unauthorized(string code, string description) =>
        new(code, description, ErrorType.Unauthorized);

    public static Error Unauthorized(string description) =>
        new("Error.Unauthorized", description, ErrorType.Unauthorized);

    public static Error InvalidId(string? entityName = null)
    {
        var prefix = string.IsNullOrWhiteSpace(entityName) ? "Entity" : entityName;
        return Validation($"{prefix}.InvalidId", "Invalid Id");
    }

    public static Error Exception(string methodName, Exception ex)
    {
        return Failure($"{methodName}.Exception", $"Exception in {methodName}: {ex.Message}");
    }

    public static Error Exception(string methodName)
    {
        return Failure($"{methodName}.Exception", $"Exception in {methodName}.");
    }

    public static Error Exception(string methodName, string className, Exception ex)
    {
        return Failure($"{className}.{methodName}.Exception", $"Exception in {className} in the function {methodName}: {ex.Message}");
    }

    public static Error FromType(ErrorType errorType, string description, string? code = null)
    {
        var resolvedCode = string.IsNullOrWhiteSpace(code)
            ? $"Error.{errorType}"
            : code;

        return errorType switch
        {
            ErrorType.Validation => Validation(resolvedCode, description),
            ErrorType.NotFound => NotFound(resolvedCode, description),
            ErrorType.Conflict => Conflict(resolvedCode, description),
            ErrorType.Unauthorized => Unauthorized(resolvedCode, description),
            ErrorType.None => None,
            _ => Failure(resolvedCode, description)
        };
    }
}
