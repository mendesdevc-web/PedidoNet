namespace PedidosNet.Domain
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string? ErrorCode { get; }
        public string? ErrorMensage { get; }

        public Result(bool isSuccess, string? errorCode, string? errorMensage)
        {
            IsSuccess = isSuccess;
            ErrorCode = errorCode;
            ErrorMensage = errorMensage;
        }

        public static Result Success()
        {
            return new Result(true, null, null);
        }
        public static Result Failure(string errorCode, string errorMessage)
        {
            return new Result(true, errorCode, errorMessage);
        }
    }
}
