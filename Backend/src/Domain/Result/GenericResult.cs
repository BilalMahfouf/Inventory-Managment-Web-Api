using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Results
{
    public class Result<T> :
        Result
    {
        public T? Value { get; }

        private Result(bool isSuccess, T value, string error, ErrorType errorType = ErrorType.NotFound)
            : base(isSuccess, error, errorType)
        {
            Value = value;
        }

        public static new Result<T> Success(T value) => new Result<T>(true, value, null!);
        public static new Result<T> Failure(string error, ErrorType errorType)
            => new Result<T>(false, default!, error, errorType);
        public static new Result<T> NotFound(string entity)
        {
            string errorMessage = $"{entity} not found";
            return Failure(errorMessage, ErrorType.NotFound);
        }
        public static new Result<T> InvalidId()
        {
            return Failure("Invalid Id", ErrorType.BadRequest);
        }

        public static new Result<T> Exception(string methodName, Exception ex)
        {
            string errorMessage = $"Exception in {methodName}: {ex.Message}";
            return Failure(errorMessage, ErrorType.InternalServerError);
        }
        public static new Result<T> Exception(
            string methodName,
            string className,
            Exception ex)
        {

            string errorMessage = $"Exception in {className} in the function {methodName}: {ex.Message}";
            return Failure(errorMessage, ErrorType.InternalServerError);
        }

    }
}
