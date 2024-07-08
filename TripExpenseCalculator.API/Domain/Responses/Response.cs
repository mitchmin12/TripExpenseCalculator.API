namespace TripExpenseCalculator.API.Domain.Responses
{
    public class Response
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string Message { get; }

        internal Response(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public static Response OK()
        {
            return new Response(true, "Success");
        }

        public static Response<T> OK<T>(T value)
        {
            return new Response<T>(value);
        }

        public static Response Fail(string error)
        {
            return new Response(false, error);
        }

        public static Response<T> Fail<T>(string error)
        {
            return new Response<T>(error);
        }
    }

    public class Response<T> : Response
    {
        public T Value { get; }

        internal Response(T value) : base(true, "Success")
        {
            Value = value;
        }

        public Response(string error) : base(false, error) { }
    }
}