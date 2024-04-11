namespace Ozon.Route256.Practice.CustomerService.ClientBalancing;

public sealed class DbStore : IDbStore
{
    private DbEndpoint[] _endpoints = Array.Empty<DbEndpoint>();
    
    
    public uint BucketsCount { get; set; }

    public Task UpdateEndpointsAsync(IReadOnlyCollection<DbEndpoint> dbEndpoints)
    {
        var endpoints = new DbEndpoint[dbEndpoints.Count];

        var i = 0;

        var bucketsCount = 0;
        foreach (var endpoint in dbEndpoints)
        {
            endpoints[i++] =  endpoint;
            bucketsCount   += endpoint.Buckets.Length;
        }

        if (BucketsCount != default && BucketsCount != bucketsCount)
            throw new InvalidOperationException("Buckets count have been changed");
        
        BucketsCount = (uint)bucketsCount;
        _endpoints   = endpoints;

        return Task.CompletedTask;
    }

    public DbEndpoint GetEndpointByBucket(
        int bucketId)
    {
        var result = _endpoints.FirstOrDefault(x => x.Buckets.Contains(bucketId));

        if (result is null)
        {
            throw new ArgumentOutOfRangeException($"There is no info about bucket {bucketId}");
        }

        return result;
    }
}