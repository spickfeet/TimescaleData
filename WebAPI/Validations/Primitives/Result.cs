namespace WebAPI.Validations.Primitives
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public T? Value { get; }
        public Fault? Error { get; private set; }

        private Result(bool isSuccess, T? value, Fault? errors)
        {
            IsSuccess = isSuccess;
            Value = value;
            if (errors != null) Error = errors;
        }

        public static Result<T> Success(T value) => new(true, value, null);

        public static Result<T> Failure(Fault error) => new(false, default, error);
    }

    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Fault? Error { get; private set; }

        private Result(bool isSuccess, Fault? errors)
        {
            IsSuccess = isSuccess;
            if (errors != null) Error = errors;
        }

        public static Result Success() => new(true, null);
        public static Result Failure(Fault error) => new(false, error);
    }
}
