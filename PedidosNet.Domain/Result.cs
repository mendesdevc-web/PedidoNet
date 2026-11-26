
namespace PedidosNet.Domain
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string? ErrorCode { get; }
        public string? ErrorMessage { get; }

        protected Result(bool isSuccess, string? errorCode, string? errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public static Result Success()
        {
            return new Result(true, null, null);
        }

        public static Result Failure(string errorCode, string errorMessage)
        {
            return new Result(false, errorCode, errorMessage);
        }
    }

    public sealed class Result<T> : Result
    {
        public T? Value { get; }

        private Result(T value)
            : base(true, null, null)
        {
            Value = value;
        }

        private Result(string errorCode, string errorMessage)
            : base(false, errorCode, errorMessage)
        {
            Value = default;
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(value);
        }

        public static new Result<T> Failure(string errorCode, string errorMessage)
        {
            return new Result<T>(errorCode, errorMessage);
        }
    }
}
