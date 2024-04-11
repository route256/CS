using System;
using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;

namespace Ozon.Route256.Postgres.Persistence.Common;

public abstract class SqlMigration : IMigration
{
    public void GetUpExpressions(IMigrationContext context)
    {
        _ = context ?? throw new ArgumentNullException(nameof(context));

        context.Expressions.Add(new ExecuteSqlStatementExpression { SqlStatement = GetUpSql(context.ServiceProvider) });
    }

    public void GetDownExpressions(IMigrationContext context)
    {
        _ = context ?? throw new ArgumentNullException(nameof(context));

        context.Expressions.Add(new ExecuteSqlStatementExpression { SqlStatement = GetDownSql(context.ServiceProvider) });
    }

    protected abstract string GetUpSql(IServiceProvider services);
    protected abstract string GetDownSql(IServiceProvider services);

    object IMigration.ApplicationContext => throw new NotSupportedException();
    string IMigration.ConnectionString => throw new NotSupportedException();
}
