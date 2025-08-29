using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Results
{
    public class Result<T> :
        Result where T : class
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
            string errorMessage = $"{entity} nor found";
            return Failure(errorMessage, ErrorType.NotFound);
        }

    }
}
