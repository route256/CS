using Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Kafka.Consumers;
using Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Kafka.Producers;

namespace Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Kafka;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafka(this IServiceCollection collection)
    {
        collection
            .AddOptions<NewOrderConsumerConfig>()
            .Configure<IConfiguration>(
                (opt, config) => 
                    config
                        .GetSection("Kafka:Consumers:NewOrder")
                        .Bind(opt));
        
        collection
            .AddOptions<KafkaProducerConfig>()
            .Configure<IConfiguration>(
                (opt, config) => 
                    config
                        .GetSection("Kafka:Producer")
                        .Bind(opt));
        
        collection
            .AddSingleton<IConsumerProvider, ConsumerProvider>()
            .AddHostedService<NewOrderConsumer>();

        collection
            .AddSingleton<IProducerProvider, ProducerProvider>()
            .AddScoped<OrderEventProducer>();

        return collection;
    }
}
