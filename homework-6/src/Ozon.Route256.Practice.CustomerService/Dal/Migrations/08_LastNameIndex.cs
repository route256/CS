using FluentMigrator;
using Ozon.Route256.Practice.CustomerService.Dal.Common;

namespace Ozon.Route256.Practice.CustomerService.Dal.Migrations;

[Migration(8, "LastNameIndex")]
public class LastNameIndex : SqlMigration
{
    protected override string GetUpSql(
        IServiceProvider services) => @"

create table idx_customers_last_name
(
    last_name   text NOT NULL,
    customer_id int  NOT NULL,
    PRIMARY KEY (last_name, customer_id)
);

";


protected override string GetDownSql(
        IServiceProvider services) => @"
    
";
}