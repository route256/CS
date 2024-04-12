namespace Ozon.Route256.Practice.CustomerService.Infrastructure.Dal.Common.Shard;

public class Shards
{
    public const string BucketPlaceholder = "__bucket__";
    
    public static string GetSchemaName(int bucketId) => $"bucket_{bucketId}";
}