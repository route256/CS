using Murmur;
using Ozon.Route256.Practice.CustomerService.ClientBalancing;

namespace Ozon.Route256.Practice.CustomerService.Dal.Common.Shard;

public class LongShardingRule: IShardingRule<long>
{
    private readonly uint _bucketsCount;

    public LongShardingRule(
        IDbStore dbStore)
    {
        _bucketsCount = dbStore.BucketsCount;
    }

    public uint GetBucketId(
        long shardKey)
    {
        var hash = GetHashCodeFromShardKey(shardKey);
        return (uint) Math.Abs(hash) % _bucketsCount;
    }

    private int GetHashCodeFromShardKey(
        long shardKey)
    {
        var bytes = BitConverter.GetBytes(shardKey);
        var murmur = MurmurHash.Create32();
        var hash = murmur.ComputeHash(bytes);
        return BitConverter.ToInt32(hash);
    }
}