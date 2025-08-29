using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Results
{
    public class Result
    {
        public bool IsSuccess { get; protected set; }
        public string? ErrorMessage { get; protected set; }
        public ErrorType ErrorType { get; protected set; }

        protected Result(bool isSuccess, string? errorMessage = null, ErrorType errorType = ErrorType.InternalServerError)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            ErrorType = errorType;
        }
        public static Result Success => new Result(true);
        public static Result Failure(string errorMessage,
            ErrorType errorType = ErrorType.InternalServerError)
        {
            return new Result(false, errorMessage, errorType);
        }
        public static Result NotFound(string entity)
        {
            string errorMessage = $"{entity} nor found";
            return Failure(errorMessage, ErrorType.NotFound);
        }
    }
}
