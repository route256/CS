using System.Diagnostics.CodeAnalysis;

namespace Ozon.Route256.Practice.LogisticsSimulator.Handlers.ResultTypes;

public class HandlerResult
{
    public HandlerException? Error { get; }
    
    [MemberNotNullWhen(false, nameof(Error))]
    public bool Success { get; }
    
    private HandlerResult(bool success, HandlerException? error)
    {
        Success = success;
        Error = error;
    }
    
    public static HandlerResult Ok => new(true, null);
    
    public static HandlerResult FromError(HandlerException error) => new(false, error);
    
    public static implicit operator HandlerResult(HandlerException exception) => new(false, exception);
}

public class HandlerResult<TValue>
{
    public TValue? Value { get; }
    public HandlerException? Error { get; }
    
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool Success { get; }

    private HandlerResult(TValue? value, bool success, HandlerException? error)
    {
        Value = value;
        Success = success;
        Error = error;
    } 
    
    private HandlerResult(TValue value): this(value, true, null)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    private HandlerResult(HandlerException innerError) : this(default, false, innerError)
    {
    }
    
    
    public static HandlerResult<TValue> FromValue(TValue payload) => new(payload);
    
    public static HandlerResult<TValue> FromError(HandlerException error) => new(error);
    
    public static implicit operator HandlerResult<TValue>(TValue payload) => new(payload);
    
    public static implicit operator HandlerResult<TValue>(HandlerException exception) =>
        new(exception);
}