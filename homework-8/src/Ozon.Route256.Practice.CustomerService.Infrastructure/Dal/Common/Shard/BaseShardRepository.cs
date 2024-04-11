using System.Data.Common;

namespace Ozon.Route256.Practice.CustomerService.Infrastructure.Dal.Common.Shard;

public class BaseShardRepository
{
    
    protected readonly IShardConnectionFactory _connectionFactory;
    protected readonly IShardingRule<long> _longShardingRule;
    private readonly IShardingRule<string> _stringShardingRule;

    public BaseShardRepository(
        IShardConnectionFactory connectionFactory,
        IShardingRule<long> longShardingRule,
        IShardingRule<string> stringShardingRule)
    {
        _connectionFactory       = connectionFactory;
        _longShardingRule        = longShardingRule;
        _stringShardingRule = stringShardingRule;
    }

    protected DbConnection GetConnectionByShardKey(
        int shardKey)
    {
        var bucketId = _longShardingRule.GetBucketId(shardKey);
        var connection = _connectionFactory.GetConnectionByBucket((int)bucketId);
        return connection;
    }   
    
    protected DbConnection GetConnectionBySearchKey(
        string searchKey)
    {
        var bucketId = _stringShardingRule.GetBucketId(searchKey);
        return _connectionFactory.GetConnectionByBucket((int)bucketId);
    }
    
    protected async Task<DbConnection> GetConnectionByBucket(
        int bucketId,
        CancellationToken token)
    {
        var connection = _connectionFactory.GetConnectionByBucket((int)bucketId);
        return connection;
    }
    
}