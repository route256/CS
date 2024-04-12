namespace Ozon.Route256.Practice.LogisticsSimulator.Handlers.OrderCancel;

public class OrderCancellationException: HandlerException
{
    public OrderCancellationException(string error): base(error)
    {
    }
}