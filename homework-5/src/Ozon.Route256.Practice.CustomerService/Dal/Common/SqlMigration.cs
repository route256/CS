using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;

namespace Ozon.Route256.Practice.CustomerService.Dal.Common;

public abstract class SqlMigration: IMigration
{
    public void GetUpExpressions(
        IMigrationContext context)
    {
        context.Expressions.Add(new ExecuteSqlStatementExpression
        {
            SqlStatement = GetUpSql(context.ServiceProvider)
        });
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