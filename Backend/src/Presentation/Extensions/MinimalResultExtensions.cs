using Domain.Shared.Errors;
using Domain.Shared.Results;

namespace Presentation.Extensions;

public static class MinimalResultExtensions
{
    public static IResult ToMinimalApiResult<T>(
        this Result<T> result,
        string? routeName = null,
        object? routeValues = null)
    {
        if (!result.IsSuccess)
        {
            return ToErrorResult(result);
        }

        var payload = ApiResponse<T>.Ok(result.Value);

        return string.IsNullOrWhiteSpace(routeName)
            ? Results.Ok(payload)
            : Results.CreatedAtRoute(routeName, routeValues, payload);
    }

    public static IResult ToMinimalApiResult(this Result result)
    {
        if (!result.IsSuccess)
        {
            return result.Problem();
        }

        return Results.Ok(ApiResponse.Ok());
    }

    private static IResult ToErrorResult(Result result)
    {
        return result.Problem();
    }

    public static IResult Problem(this Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException();
        }

        if (result.Error is null)
        {
            throw new ArgumentNullException();
        }

        return Results.Problem(
            title: GetTitle(result.Error),
            detail: GetDetail(result.Error),
            type: GetType(result.Error.Type),
            statusCode: GetStatusCode(result.Error.Type),
            extensions: GetErrors(result));

        static string GetTitle(Error error) =>
            error.Type switch
            {
                ErrorType.Validation => error.Code,
                ErrorType.Unauthorized => error.Code,
                ErrorType.NotFound => error.Code,
                ErrorType.Conflict => error.Code,
                _ => "Server failure"
            };

        static string GetDetail(Error error) =>
            error.Type switch
            {
                ErrorType.Validation => error.Description,
                ErrorType.Unauthorized => error.Description,
                ErrorType.NotFound => error.Description,
                ErrorType.Conflict => error.Description,
                _ => "An unexpected error occurred"
            };

        static string GetType(ErrorType errorType) =>
            errorType switch
            {
                ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                ErrorType.Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1",
                ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };

        static int GetStatusCode(ErrorType errorType) =>
            errorType switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            };

        static Dictionary<string, object?> GetErrors(Result failedResult)
        {
            return new Dictionary<string, object?>
            {
                {
                    "errors",
                    new[]
                    {
                        failedResult.Error.Code,
                        failedResult.Error.Description
                    }
                }
            };
        }
    }
}
