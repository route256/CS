using Ozon.Route256.Practice.LogisticsSimulator.DateTimeProvider;
using Ozon.Route256.Practice.LogisticsSimulator.Handlers.ResultTypes;
using Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Repositories;
using Ozon.Route256.Practice.LogisticsSimulator.Model;

namespace Ozon.Route256.Practice.LogisticsSimulator.Handlers.OrderCancel;

public class OrderCancellationHandler : IOrderCancellationHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    private static readonly HashSet<OrderState> ForbiddenToCancelStates = new()
    {
        OrderState.Cancelled,
        OrderState.Delivered
    };

    public OrderCancellationHandler(IOrderRepository orderRepository, IDateTimeProvider dateTimeProvider)
    {
        _orderRepository = orderRepository;
        _dateTimeProvider = dateTimeProvider;
    }
    

    public async Task<HandlerResult> Handle(IOrderCancellationHandler.Request request, CancellationToken token)
    {
        var order = await _orderRepository.Find(request.OrderId, token);

        if (order is null)
            return HandlerResult.FromError(new OrderCancellationException($"Order with Id:{request.OrderId} not found"));
        
        if(ForbiddenToCancelStates.Contains(order.Value.OrderState))
            return HandlerResult.FromError(new OrderCancellationException($"Cannot cancel order {request.OrderId} in state {order.Value.OrderState.ToString()}"));

        var cancelledOrder = order.Value with
        {
            OrderState = OrderState.Cancelled, ChangedAt = _dateTimeProvider.CurrentDateTimeOffsetUtc
        };

        await _orderRepository.Update(cancelledOrder, token);
        return HandlerResult.Ok;
    }
}