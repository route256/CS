using System;

namespace Ozon.Route256.Postgres.Domain;

public sealed record Order(long Id, long ClientId, OrderState State, decimal Amount, DateTimeOffset Date, Order.Item[] Items)
{
    public sealed record Item(long SkuId, int Quantity, decimal Price);
}
