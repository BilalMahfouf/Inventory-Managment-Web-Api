namespace Presentation.Contracts;

public record ApiResponse(bool Success, string Message)
{
    public static ApiResponse Ok(string message = "Request completed successfully") =>
        new(true, message);
}

public record ApiResponse<T>(bool Success, string Message, T Data) : ApiResponse(Success, Message)
{
    public static ApiResponse<T> Ok(T data, string message = "Request completed successfully") =>
        new(true, message, data);
}
