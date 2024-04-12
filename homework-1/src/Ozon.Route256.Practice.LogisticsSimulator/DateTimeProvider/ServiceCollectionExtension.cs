using Ozon.Route256.Practice.LogisticsSimulator.Handlers.OrderCancel;
using Ozon.Route256.Practice.LogisticsSimulator.Handlers.OrderRegistration;
using Ozon.Route256.Practice.LogisticsSimulator.Handlers.OrdersRandomUpdate;

namespace Ozon.Route256.Practice.LogisticsSimulator.DateTimeProvider;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddDateTimeProvider(this IServiceCollection collection)
    {
        collection.AddSingleton<IDateTimeProvider, LocalDateTimeProvider>();
        return collection;
    }
}