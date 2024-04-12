using FluentMigrator;
using Ozon.Route256.Practice.CustomerService.Dal.Common;

namespace Ozon.Route256.Practice.CustomerService.Dal.Migrations;

[Migration(
    7,
    "AddressType")]
public class AddressType : SqlMigration
{
    protected override string GetUpSql(
        IServiceProvider services) => @"

do $$
begin
    if not exists (select 1 from pg_type where typname = 'address') then        
        create type public.address as (
    id int,
    customer_id int,
    is_default bool,
    region text,
    city text,
    street text,
    building text,
    apartment text,
    latitude numeric,
    longitude numeric
        );
    end if;
end $$

";


protected override string GetDownSql(
        IServiceProvider services) => @"
    
    drop type public.customer; 

";
}