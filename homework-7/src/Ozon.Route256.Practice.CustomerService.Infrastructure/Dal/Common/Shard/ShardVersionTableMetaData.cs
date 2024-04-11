using FluentMigrator.Runner.VersionTableInfo;

namespace Ozon.Route256.Practice.CustomerService.Infrastructure.Dal.Common.Shard;

public class ShardVersionTableMetaData: IVersionTableMetaData
{
    private readonly BucketMigrationContext _context;

    public ShardVersionTableMetaData(
        BucketMigrationContext context)
    {
        _context = context;
    }

    public object ApplicationContext { get; set; }
    public bool OwnsSchema => true;
    public string SchemaName => _context.CurrentSchema;
    public string TableName => "version_info";
    public string ColumnName => "version";
    public string DescriptionColumnName => "description";
    public string UniqueIndexName => "version_unq_idx";
    public string AppliedOnColumnName => "applied_on";
}