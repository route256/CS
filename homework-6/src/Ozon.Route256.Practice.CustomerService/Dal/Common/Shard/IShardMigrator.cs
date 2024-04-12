namespace Ozon.Route256.Practice.CustomerService.Dal.Common.Shard;

public interface IShardMigrator
{
    Task Migrate(
        CancellationToken token);
}