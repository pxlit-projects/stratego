namespace Stratego.Common
{
    //DO NOT TOUCH THIS FILE!

    public class Result
    {
        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public string Message { get; }

        protected Result(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public static Result CreateSuccessResult()
        {
            return new Result(true, string.Empty);
        }

        public static Result CreateFailureResult(string message)
        {
            return new Result(false, message);
        }

    }

    public class Result<T>
    {
        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public string Message { get; }

        public T Value { get; }

        protected Result(bool isSuccess, string message, T value)
        {
            IsSuccess = isSuccess;
            Message = message;
            Value = value;
        }

        public static Result<T> CreateSuccessResult(T value)
        {
            return new Result<T>(true, string.Empty, value);
        }

        public static Result<T> CreateFailureResult(string message)
        {
            return new Result<T>(false, message, default);
        }

    }
}