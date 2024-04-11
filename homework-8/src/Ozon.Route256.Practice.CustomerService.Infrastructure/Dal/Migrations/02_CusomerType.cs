using FluentMigrator;
using Ozon.Route256.Practice.CustomerService.Infrastructure.Dal.Common;

namespace Ozon.Route256.Practice.CustomerService.Dal.Migrations;

[Migration(2, "Customer type")]
public class CustomerType: SqlMigration
{
    protected override string GetUpSql(
        IServiceProvider services) => @"


create type customer as (
    id int,
    first_name text,
    last_name text,
    mobile_number text,
    email text,
    address_id int,
    addresses int[]
);

";

    protected override string GetDownSql(
        IServiceProvider services) => @"
    
    drop type customer; 

";
}