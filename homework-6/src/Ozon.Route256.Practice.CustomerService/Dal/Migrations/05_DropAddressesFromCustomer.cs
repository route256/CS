using FluentMigrator;
using Ozon.Route256.Practice.CustomerService.Dal.Common;

namespace Ozon.Route256.Practice.CustomerService.Dal.Migrations;

[Migration(5, "Drop addresses from customer")]
public class DropAddressesFromCustomer: SqlMigration {
    
    protected override string GetUpSql(
        IServiceProvider services) => @"

alter table customers
    drop column address_id cascade , 
    drop column addresses;

";

    protected override string GetDownSql(
        IServiceProvider services) => @"
    
alter table customers
    add column address_id int, 
    add column addresses int[];

";
}