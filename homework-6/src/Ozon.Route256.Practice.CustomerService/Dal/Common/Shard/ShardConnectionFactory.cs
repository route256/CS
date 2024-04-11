using System.Data.Common;
using Microsoft.Extensions.Options;
using Npgsql;
using Ozon.Route256.Practice.CustomerService.ClientBalancing;

namespace Ozon.Route256.Practice.CustomerService.Dal.Common.Shard;


public interface IShardConnectionFactory
{
    DbConnection GetConnectionByBucket(int bucketId);

    IEnumerable<int> GetAllBuckets();
}

public class ShardConnectionFactory: IShardConnectionFactory
{
    private readonly IDbStore _dbStore;
    private readonly DbOptions _dbOptions;

    public ShardConnectionFactory(
        IDbStore dbStore,
        IOptions<DbOptions> dbOptions)
    {
        _dbStore   = dbStore;
        _dbOptions = dbOptions.Value;
    }
    

    public IEnumerable<int> GetAllBuckets()
    {
        for (var bucketId = 0; bucketId < _dbStore.BucketsCount; bucketId++)
        {
            yield return bucketId;
        }
    }

    public DbConnection GetConnectionByBucket(int bucketId)
    {
        var endpoint = _dbStore.GetEndpointByBucket(bucketId);
        var connectionString = GetConnectionString(endpoint);
        return new ShardNpgsqlConnection(new NpgsqlConnection(connectionString), bucketId);
    }

    private string GetConnectionString(DbEndpoint endpoint)
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = endpoint.HostAndPort,
            Database = _dbOptions.DatabaseName,
            Username = _dbOptions.User,
            Password = _dbOptions.Password
        };
        return builder.ToString();
    }
}