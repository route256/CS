using Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Repositories;
using Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.BackgroundJobs;
using Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Grpc;
using Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Kafka;
using Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Redis;

namespace Ozon.Route256.Practice.LogisticsSimulator.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHostedService<UpdateOrderJob>();
        services
            .AddRepository()
            .AddKafka()
            .AddRedis(configuration)
            .AddGrpcReflection()
            .AddGrpc(o => o.Interceptors.Add<ExceptionInterceptor>());

        return services;
    }
}
