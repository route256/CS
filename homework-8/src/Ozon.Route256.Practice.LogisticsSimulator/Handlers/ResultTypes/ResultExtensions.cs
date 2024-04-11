using System.Runtime.ExceptionServices;

namespace Ozon.Route256.Practice.LogisticsSimulator.Handlers.ResultTypes;

public static class ResultExtensions
{
    /// <summary>
    /// Returns different values, depend on <see cref="HandlerResult"/> result
    /// </summary>
    /// <param name="handlerResult"></param>
    /// <param name="successDelegate">Return if success</param>
    /// <param name="failedDelegate">Return if failed</param>
    /// <returns></returns>
    public static TResult Handle<TResult>(this HandlerResult handlerResult, Func<TResult> successDelegate, Func<HandlerException, TResult> failedDelegate) => 
        handlerResult.Success ? successDelegate() : failedDelegate(handlerResult.Error);

    /// <summary>
    /// Returns different values, depend on <see cref="HandlerResult"/> result
    /// </summary>
    /// <param name="handlerResult"></param>
    /// <param name="successDelegate">Return if success</param>
    /// <param name="failedDelegate">Return if failed</param>
    /// <returns></returns>
    public static TResult Handle<TResult, T>(this HandlerResult<T> handlerResult, Func<T, TResult> successDelegate, Func<HandlerException, TResult> failedDelegate) where T : class => 
        handlerResult.Success ? successDelegate(handlerResult.Value) : failedDelegate(handlerResult.Error);

    /// <summary>
    /// Checks if the <see cref="HandlerResult"/> was not successful and throws the underlying exception, if available.
    /// While re-throwing, it preserves the StackTrace. Returns the Result if successful, so that it can be quickly checked.
    /// </summary>
    /// <param name="handlerResult"></param>
    public static HandlerResult Check(this HandlerResult handlerResult)
    {
        if (handlerResult.Success) 
            return handlerResult;
        var error = handlerResult.Error;
        PreserveStackTrace(error);
        throw error;
    }

    /// <summary>
    /// Checks if the <see cref="HandlerResult"/> was not successful and throws the underlying exception, if available.
    /// While re-throwing, it preserves the StackTrace. Returns the Result if successful, so that it can be quickly checked.
    /// </summary>
    /// <param name="handlerResult"></param>
    public static HandlerResult<T> Check<T>(this HandlerResult<T> handlerResult) where T : class
    {
        if (handlerResult.Success) 
            return handlerResult;
        
        var error = handlerResult.Error;
        PreserveStackTrace(error);
        throw error;
    }
    
    private static void PreserveStackTrace(Exception exception)
    {
        ExceptionDispatchInfo.Capture(exception).Throw();
    }
}