using Confluent.Kafka;

namespace Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Kafka.Consumers;

public class NewOrderConsumerConfig
{
    public string Topic { get; init; } = null!;
    public ConsumerConfig Config { get; init; } = new();
}
