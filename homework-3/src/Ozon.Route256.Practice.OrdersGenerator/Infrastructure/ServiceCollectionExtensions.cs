using Ozon.Route256.Practice.OrdersGenerator.Infrastructure.Kafka;
using Ozon.Route256.Practice.OrdersGenerator.Infrastructure.Workers;

namespace Ozon.Route256.Practice.OrdersGenerator.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddHostedService<OrdersGeneratorWorker>();
        services.AddScoped<IKafkaProducer, KafkaProducer>();

        return services;
    }
}