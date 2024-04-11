using FluentMigrator;
using Ozon.Route256.Practice.CustomerService.Infrastructure.Dal.Common;

namespace Ozon.Route256.Practice.CustomerService.Dal.Migrations;

[Migration(4, "FillCustomerAndDefaultForAddresses")]
public class FillCustomerAndDefaultForAddresses : SqlMigration
{
    protected override string GetUpSql(
        IServiceProvider services) => @"

update addresses a
set customer_id = c.id,
    is_default = a.id = c.address_id
from customers c 
where a.id = any (c.addresses) 
   or a.id = c.address_id;

";

    protected override string GetDownSql(
        IServiceProvider services) => @"
    
";
}