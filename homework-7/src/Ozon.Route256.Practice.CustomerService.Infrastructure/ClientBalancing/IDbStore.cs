namespace Ozon.Route256.Practice.CustomerService.Infrastructure.ClientBalancing;

public interface IDbStore
{
    Task UpdateEndpointsAsync(IReadOnlyCollection<DbEndpoint> dbEndpoints);

    DbEndpoint GetEndpointByBucket(
        int bucketId);
    
    uint BucketsCount { get; }
}