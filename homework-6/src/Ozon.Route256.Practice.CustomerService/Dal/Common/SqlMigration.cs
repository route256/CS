using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using Ozon.Route256.Practice.CustomerService.Dal.Common.Shard;

namespace Ozon.Route256.Practice.CustomerService.Dal.Common;

public abstract class SqlMigration: IMigration
{
    public void GetUpExpressions(
        IMigrationContext context)
    {
        var sqlStatement = GetUpSql(context.ServiceProvider);

        var bucketContext = context.ServiceProvider.GetRequiredService<BucketMigrationContext>();
        var currentSchema = bucketContext.CurrentSchema;

        if (!context.QuerySchema.SchemaExists(currentSchema))
        {
            context.Expressions.Add(new ExecuteSqlStatementExpression { SqlStatement = $"create schema {currentSchema};"});
        }
        
        context.Expressions.Add(new ExecuteSqlStatementExpression { SqlStatement = $"set search_path to {currentSchema};"});
        context.Expressions.Add(new ExecuteSqlStatementExpression {SqlStatement = sqlStatement});
    }

    public void GetDownExpressions(
        IMigrationContext context)
    {
        context.Expressions.Add(new ExecuteSqlStatementExpression
        {
            SqlStatement = GetDownSql(context.ServiceProvider)
        });
    }

    public object ApplicationContext { get; }
    public string ConnectionString { get; }

    protected abstract string GetUpSql(
        IServiceProvider services);
    protected abstract string GetDownSql(
        IServiceProvider services);
}