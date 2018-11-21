namespace HZC.Infrastructure
{
    public class Result<T> : Result
    {
        public T Body { get; set; }

        public Result()
        { }

        public Result(int code, T body, string message = "")
        {
            Code = code;
            Body = body;
            Message = message;
        }
    }
}