namespace Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Repositories;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepository(this IServiceCollection collection)
    {
        collection.AddScoped<IOrderRepository, OrderInMemoryRepository>();

        return collection;
    }
}