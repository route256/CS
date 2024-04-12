using FluentMigrator;
using Ozon.Route256.Practice.CustomerService.Infrastructure.Dal.Common;

namespace Ozon.Route256.Practice.CustomerService.Dal.Migrations;

[Migration(6, "Add customr id index for addresses")]
public class AddCustomrIdIndexForAddresses: SqlMigration {
    
    protected override string GetUpSql(
        IServiceProvider services) => @"

create index customer_id_idx on addresses (customer_id);

";

    protected override string GetDownSql(
        IServiceProvider services) => @"
    
drop index customer_id_idx;

";
}