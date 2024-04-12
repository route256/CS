namespace Ozon.Route256.Practice.OrdersGenerator.Generator;

public interface IOrderGenerator
{
    Task GenerateOrder(
        CancellationToken token);
}