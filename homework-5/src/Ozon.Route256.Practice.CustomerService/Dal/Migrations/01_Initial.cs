using FluentMigrator;
using Ozon.Route256.Practice.CustomerService.Dal.Common;

namespace Ozon.Route256.Practice.CustomerService.Dal.Migrations;

[Migration(1, "Initial migration")]
public class Initial: SqlMigration
{
    protected override string GetUpSql(
        IServiceProvider services) => @"

create table addresses(
    id serial primary key,
    region text,
    city text,
    street text,
    building text,
    apartment text,
    latitude numeric,
    longitude numeric
);

create table customers(
    id serial primary key,
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
    
    drop table customers;
    drop table addresses; 

";
}