namespace Ozon.Route256.Practice.CustomerService.ClientBalancing;

public interface IDbStore
{
    Task UpdateEndpointsAsync(IReadOnlyCollection<DbEndpoint> dbEndpoints);
}

public sealed class DbStore : IDbStore
{
    private DbEndpoint[] _endpoints = Array.Empty<DbEndpoint>();

    public Task UpdateEndpointsAsync(IReadOnlyCollection<DbEndpoint> dbEndpoints)
    {
        var endpoints = new DbEndpoint[dbEndpoints.Count];

        var i = 0;

        foreach (var endpoint in dbEndpoints)
        {
            endpoints[i++] = endpoint;
        }

        _endpoints = endpoints;

        return Task.CompletedTask;
    }
}