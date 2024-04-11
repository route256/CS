namespace Ozon.Route256.Practice.CustomerService.Infrastructure.Dal.Common.Shard;

public interface IShardingRule<TShardKey>
{
    uint GetBucketId(
        TShardKey shardKey);
}