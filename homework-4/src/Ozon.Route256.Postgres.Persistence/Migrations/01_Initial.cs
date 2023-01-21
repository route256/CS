using System;
using FluentMigrator;
using Ozon.Route256.Postgres.Persistence.Common;

namespace Ozon.Route256.Postgres.Persistence.Migrations;

[Migration(1, "Initial migration")]
public sealed class Initial : SqlMigration
{
    protected override string GetUpSql(IServiceProvider services) => @"
CREATE TYPE order_state AS ENUM
(
  'unknown',
  'created',
  'paid',
  'boxing',
  'wait_for_pickup',
  'in_delivery',
  'wait_for_client',
  'completed',
  'cancelled'
);

CREATE TABLE orders
(
    order_id     bigint         PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    client_id    bigint         NOT NULL,
    order_state  order_state    NOT NULL,
    order_amount numeric(12,2)  NOT NULL,
    order_date   timestamptz    NOT NULL
);

CREATE INDEX ON orders (client_id);

CREATE TABLE order_items
(
    order_id    bigint          NOT NULL REFERENCES orders (order_id),
    sku_id      bigint          NOT NULL,
    quantity    integer         NOT NULL,
    price       numeric(12,2)   NOT NULL,

    PRIMARY KEY (order_id, sku_id)
);
";

    protected override string GetDownSql(IServiceProvider services) => @"
DROP TABLE order_items;
DROP TABLE orders;
DROP TYPE order_state;
";
}
