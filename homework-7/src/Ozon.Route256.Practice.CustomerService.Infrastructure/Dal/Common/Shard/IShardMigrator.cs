namespace Ozon.Route256.Practice.CustomerService.Infrastructure.Dal.Common.Shard;

public interface IShardMigrator
{
    Task Migrate(
        CancellationToken token);
}