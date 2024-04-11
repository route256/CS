using FluentMigrator;
using Ozon.Route256.Practice.CustomerService.Dal.Common;

namespace Ozon.Route256.Practice.CustomerService.Dal.Migrations;

[Migration(3, "AddCustomerAndDefaultToAddresses")]
public class AddCustomerAndDefaultToAddresses : SqlMigration 
{
    protected override string GetUpSql(
        IServiceProvider services) => @"

alter table addresses
    add column customer_id int,
    add column is_default bool;

";

    protected override string GetDownSql(
        IServiceProvider services) =>
        throw new NotImplementedException();
}