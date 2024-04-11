namespace Ozon.Route256.Practice.CustomerService.Infrastructure.Dal.Common.Shard;

public class BucketMigrationContext
{
    private string _currentSchema = string.Empty;

    public void UpdateCurrentSchema(
        int bucket) => _currentSchema = Shards.GetSchemaName(bucket);

    public string CurrentSchema
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_currentSchema))
                throw new InvalidOperationException("Current db schema is not found");

            return _currentSchema;
        }
    }   
}