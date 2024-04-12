using Ozon.Route256.Practice.LogisticsSimulator.Handlers.OrderCancel;
using Ozon.Route256.Practice.LogisticsSimulator.Handlers.OrderRegistration;
using Ozon.Route256.Practice.LogisticsSimulator.Handlers.OrdersRandomUpdate;

namespace Ozon.Route256.Practice.LogisticsSimulator.Handlers;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddHandlers(this IServiceCollection collection)
    {
        collection.AddScoped<IOrderCancellationHandler, OrderCancellationHandler>();
        collection.AddScoped<IOrderRegistrationHandler, OrderRegistrationHandler>();
        collection.AddScoped<IOrdersRandomUpdateHandler, OrdersRandomUpdateHandler>();
        return collection;
    }
}