using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using Ozon.Route256.Postgres.Domain;
using Ozon.Route256.Postgres.Domain.Common;

namespace Ozon.Route256.Postgres.Persistence;

public sealed class OrderRepository : IOrderRepository
{
    private readonly string _connectionString;

    public OrderRepository(string connectionString) => _connectionString = connectionString;

    public async IAsyncEnumerable<Order> Get(
        long[] orderIds,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (orderIds.Length == 0)
            yield break;

        const string query = @"
SELECT order_id,
       client_id,
       order_state,
       order_amount,
       order_date,
       sku_id,
       quantity,
       price
FROM orders
JOIN order_items USING (order_id)
WHERE order_id = ANY(:ids)
ORDER BY order_id;
";

        await using var connection = new NpgsqlConnection(_connectionString);
        await using var command = new NpgsqlCommand(query, connection)
        {
            Parameters =
            {
                { "ids", orderIds },
            }
        };

        await connection.OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken);

        OrderRow last = default;
        var items = new List<Order.Item>();
        while (await reader.ReadAsync(cancellationToken))
        {
            var current = new OrderRow(
                reader.GetFieldValue<long>(0),
                reader.GetFieldValue<long>(1),
                reader.GetFieldValue<OrderState>(2),
                reader.GetFieldValue<decimal>(3),
                reader.GetFieldValue<DateTimeOffset>(4));

            if (current.OrderId != last.OrderId && items.Count > 0)
            {
                yield return new(last.OrderId, last.ClientId, last.State, last.Amount, last.Date, items.ToArray());
                items.Clear();
            }

            last = current;
            items.Add(
                new(
                    reader.GetFieldValue<long>(5),
                    reader.GetFieldValue<int>(6),
                    reader.GetFieldValue<decimal>(7)));
        }

        if (items.Count > 0)
            yield return new(last.OrderId, last.ClientId, last.State, last.Amount, last.Date, items.ToArray());
    }

    public async ValueTask Add(Order[] orders, CancellationToken cancellationToken)
    {
        const string query = @"
INSERT INTO orders(client_id, order_state, order_amount, order_date)
SELECT client_id, order_state, order_amount, order_date
FROM unnest(
    :client_ids,
    :order_states,
    :order_amounts,
    :order_dates
) AS source (client_id, order_state, order_amount, order_date)
RETURNING order_id;
";

        const string itemsQuery = @"
INSERT INTO order_items(order_id, sku_id, quantity, price)
SELECT order_id, sku_id, quantity, price
FROM unnest(
    :order_ids,
    :sku_ids,
    :quantities,
    :prices
) AS source (order_id, sku_id, quantity, price)
";
        if (orders.Length == 0)
            return;

        await using var connection = new NpgsqlConnection(_connectionString);
        await using var command = new NpgsqlCommand(query, connection)
        {
            Parameters =
            {
                { "client_ids", orders.ToArrayBy(o => o.ClientId) },
                { "order_states", orders.ToArrayBy(o => o.State) },
                { "order_amounts", orders.ToArrayBy(o => o.Amount) },
                { "order_dates", orders.ToArrayBy(o => o.Date) },
            }
        };

        await connection.OpenAsync(cancellationToken);
        await using var tx = await connection.BeginTransactionAsync(cancellationToken);

        var insertedOrders = new List<Order>(orders.Length);
        await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
        {
            int i = 0;
            while (await reader.ReadAsync(cancellationToken))
                insertedOrders.Add(orders[i++] with { Id = reader.GetFieldValue<long>(0) });
        }

        await using var itemsCommand = new NpgsqlCommand(itemsQuery, connection)
        {
            Parameters =
            {
                { "order_ids", insertedOrders.SelectMany(o => o.Items, (o, _) => o.Id).ToArray() },
                { "sku_ids", insertedOrders.SelectMany(o => o.Items, (_, item) => item.SkuId).ToArray() },
                { "quantities", insertedOrders.SelectMany(o => o.Items, (_, item) => item.Quantity).ToArray() },
                { "prices", insertedOrders.SelectMany(o => o.Items, (_, item) => item.Price).ToArray() },
            }
        };

        await itemsCommand.ExecuteNonQueryAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);
    }

    private readonly record struct OrderRow(long OrderId, long ClientId, OrderState State, decimal Amount, DateTimeOffset Date);
}
