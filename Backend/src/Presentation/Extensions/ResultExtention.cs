using Application.Results;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Presentation.Extensions
{


    public static class ResultExtensions
    {
        /// <summary>
        /// Converts a <see cref="Result{T}"/> to an appropriate <see cref="ActionResult{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value (must be a struct/value type).</typeparam>
        /// <param name="result">The operation result containing either a value or error.</param>
        /// <param name="actionName">
        /// When specified, returns <see cref="CreatedAtActionResult"/> (for POST methods).
        /// When null, returns <see cref="OkObjectResult"/> (for GET).
        /// </param>
        /// <param name="routeValues">Route values for location header (required if createdActionName is provided).</param>
        /// <returns>
        /// <see cref="CreatedAtActionResult"/> if createdActionName is provided,
        /// <see cref="OkObjectResult"/> for standard success,
        /// or an appropriate error response if failed.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if createdActionName is provided but routeValues is null.
        /// </exception>
        public static ActionResult<T> HandleResult<T>(
            this Result<T> result,
            string? routeName = null,
            object? routeValues = null) 
        {
            if (!result.IsSuccess)
                return HandleError(result);

            return string.IsNullOrEmpty(routeName)
                ? new OkObjectResult(result.Value)
                : new CreatedAtRouteResult(routeName, routeValues, result.Value);
        }

        /// <summary>
        /// Converts a <see cref="Result{T}"/> of <see cref="IReadOnlyCollection{T}"/> containing a collection to an appropriate <see cref="ActionResult"/>.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection (must be a struct/value type).</typeparam>
        /// <param name="result">The operation result containing either a collection or error.</param>
        /// <returns>
        /// <see cref="OkObjectResult"/> with the collection if successful,
        /// or an appropriate error response if failed.
        /// </returns>
        public static ActionResult HandleResult<T>(this Result<IReadOnlyCollection<T>> result) where T : struct
        {
            return result.IsSuccess ? new OkObjectResult(result.Value) : HandleError(result);
        }

        /// <summary>
        /// Converts a non-generic <see cref="Result"/> (for void operations) to an appropriate <see cref="ActionResult"/>.
        /// </summary>
        /// <param name="result">The operation result indicating success or failure.</param>
        /// <returns>
        /// <see cref="NoContentResult"/> if successful,
        /// or an appropriate error response if failed.
        /// </returns>
        public static ActionResult HandleResult(this Result result)
        {
            return result.IsSuccess ? new NoContentResult() : HandleError(result);
        }

        /// <summary>
        /// Handles conversion of failed results to appropriate error responses.
        /// </summary>
        /// <param name="result">The failed operation result.</param>
        /// <returns>
        /// <see cref="NotFoundObjectResult"/> for <see cref="ErrorType.NotFound"/>,
        /// <see cref="BadRequestObjectResult"/> for <see cref="ErrorType.BadRequest"/>,
        /// <see cref="ConflictObjectResult"/> for <see cref="ErrorType.Conflict"/>,
        /// or <see cref="ObjectResult"/> with 500 status code for other errors.
        /// </returns>
        private static ActionResult HandleError(Result result)
        {
            return result.ErrorType switch
            {
                ErrorType.NotFound => new NotFoundObjectResult(result.ErrorMessage),
                ErrorType.BadRequest => new BadRequestObjectResult(result.ErrorMessage),
                ErrorType.Conflict => new ConflictObjectResult(result.ErrorMessage),
                ErrorType.Unauthorized => new UnauthorizedObjectResult(result.ErrorMessage),
                _ => new ObjectResult(result.ErrorMessage) { StatusCode = 500 }
            };
        }
    }
}
