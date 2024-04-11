namespace RateLimiterCore;

public class RateLimiter<T> : IRateLimiter<T>, IDisposable
{
    public async Task<Result<T>> Invoke(Func<Task<T>> action, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}