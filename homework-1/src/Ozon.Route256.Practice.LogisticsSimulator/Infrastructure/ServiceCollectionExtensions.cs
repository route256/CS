using Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Repositories;
using Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.BackgroundJobs;
using Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Grpc;
using Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Kafka;

namespace Ozon.Route256.Practice.LogisticsSimulator.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddHostedService<UpdateOrderJob>();
        services
            .AddRepository()
            .AddKafka()
            .AddGrpcReflection()
            .AddGrpc(o => o.Interceptors.Add<ExceptionInterceptor>());

        return services;
    }
}