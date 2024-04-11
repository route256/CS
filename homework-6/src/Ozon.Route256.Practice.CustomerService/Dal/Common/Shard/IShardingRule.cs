namespace Ozon.Route256.Practice.CustomerService.Dal.Common.Shard;

public interface IShardingRule<TShardKey>
{
    uint GetBucketId(
        TShardKey shardKey);
}