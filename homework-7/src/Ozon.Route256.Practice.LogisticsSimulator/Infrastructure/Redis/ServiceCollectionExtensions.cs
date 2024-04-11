namespace Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Redis;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddRedis(
        this IServiceCollection collection,
        IConfiguration configuration)
    {
        var connectionString = configuration
            .GetSection("Redis")
            .GetValue<string>("ConnectionString");
        
        collection
            .AddScoped<IRedisDatabaseFactory>(_ => 
                new RedisDatabaseFactory(connectionString));

        return collection;
    }
}
