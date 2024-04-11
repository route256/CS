using Ozon.Route256.Practice.LogisticsSimulator.DateTimeProvider;
using Ozon.Route256.Practice.LogisticsSimulator.Handlers.ResultTypes;
using Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Repositories;
using Ozon.Route256.Practice.LogisticsSimulator.Model;

namespace Ozon.Route256.Practice.LogisticsSimulator.Handlers.OrdersRandomUpdate;

public class OrdersRandomUpdateHandler : IOrdersRandomUpdateHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly Random _random = new();
    private readonly IDateTimeProvider _dateTimeProvider;
    
    private static readonly Dictionary<OrderState, OrderState[]> AvailableStates = new ()
    {
        { OrderState.Created, new [] { OrderState.SentToCustomer } },
        { OrderState.SentToCustomer, new [] { OrderState.Delivered, OrderState.Lost } },
        { OrderState.Lost, new [] { OrderState.SentToCustomer } }
    };

    public OrdersRandomUpdateHandler(IOrderRepository orderRepository, IDateTimeProvider dateTimeProvider)
    {
        _orderRepository = orderRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<HandlerResult> Handle(CancellationToken token)
    {
        var allOrders = await _orderRepository.GetAll(token);
        if (allOrders.Count == 0)
            return HandlerResult.Ok;
        
        var ordersToUpdate = GetOrdersToUpdate(allOrders);
        var updatedOrders = UpdateOrders(ordersToUpdate);

        await _orderRepository.UpdateMany(updatedOrders, token);
        
        return HandlerResult.Ok;
    }

    private IEnumerable<Order> GetOrdersToUpdate(ICollection<Order> allOrders)
    {
        if (allOrders.Count == 1)
            return allOrders;
        
        var from = _random.Next(0, allOrders.Count - 1);
        var maxTake = allOrders.Count - from;
        var to = maxTake == 1 
            ? 1 
            :  _random.Next(1, allOrders.Count - from);
        return allOrders.Skip(from).Take(to);
    }

    private IEnumerable<Order> UpdateOrders(IEnumerable<Order> orders)
    {
        foreach (var order in orders)
        {
            if(!AvailableStates.TryGetValue(order.OrderState, out var newStates))
                continue;

            var newState = GetRandomOrderState(newStates);
            yield return order with { OrderState = newState, ChangedAt = _dateTimeProvider.CurrentDateTimeOffsetUtc};
        }
    }

    private OrderState GetRandomOrderState(IReadOnlyList<OrderState> orderStates)
    {
        if (orderStates.Count == 1)
            return orderStates[0];

        var random = _random.Next(0, orderStates.Count);
        return orderStates[random];
    }
}