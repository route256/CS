namespace Ozon.Route256.Practice.LogisticsSimulator.Handlers.OrderCancel;

public interface IOrderCancellationHandler: IHandler<IOrderCancellationHandler.Request>
{
    public record Request(long OrderId);
}