namespace RateLimiterCore;

public interface IRateLimiter<T>
{
    Task<Result<T>> Invoke(Func<Task<T>> action, CancellationToken cancellationToken);
}