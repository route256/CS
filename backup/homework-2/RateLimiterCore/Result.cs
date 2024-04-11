namespace RateLimiterCore;

public class Result<T>
{
    public bool IsLimited { get; }
	
    public T? Value { get; }
	
    public Result(T value)
    {
        Value = value;
        IsLimited = false;
    }
	
    public Result() => IsLimited = true;
	
    public static Result<T> Success(T value) => new(value);
	
    public static Result<T> Fail() => new();
}