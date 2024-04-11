using System;
using System.Linq;
using Ozon.Route256.Postgres.Grpc;

namespace Ozon.Route256.Postgres.Api.Mapping;

internal static class OrderMapping
{
    public static Order Map(this Domain.Order order) =>
        new Order
        {
            Id = order.Id,
            Amount = order.Amount.ToMoney(),
            State = order.State.ToGrpc(),
            Items = { order.Items.Select(Map) }
        };

    private static Order.Types.Item Map(this Domain.Order.Item item) =>
        new Order.Types.Item
        {
            SkuId = item.SkuId,
            Quantity = item.Quantity,
            Price = item.Price.ToMoney()
        };

    private static OrderState ToGrpc(this Domain.OrderState state) =>
        state switch {
            Domain.OrderState.Unknown => OrderState.Unknown,
            Domain.OrderState.Created => OrderState.Created,
            Domain.OrderState.Paid => OrderState.Paid,
            Domain.OrderState.Boxing => OrderState.Boxing,
            Domain.OrderState.WaitForPickup => OrderState.WaitForPickup,
            Domain.OrderState.InDelivery => OrderState.InDelivery,
            Domain.OrderState.WaitForClient => OrderState.WaitForClient,
            Domain.OrderState.Completed => OrderState.Completed,
            Domain.OrderState.Cancelled => OrderState.Cancelled,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
}
