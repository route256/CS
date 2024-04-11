using Confluent.Kafka;

namespace Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Kafka.Producers;

public class KafkaProducerConfig
{
    public ProducerConfig Config { get; init; } = new();
    public string OrderEventTopic { get; init; } = null!;
}
