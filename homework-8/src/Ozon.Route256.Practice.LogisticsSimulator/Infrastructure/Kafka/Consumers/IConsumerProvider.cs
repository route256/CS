using Confluent.Kafka;

namespace Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Kafka.Consumers;

public interface IConsumerProvider
{
    IConsumer<string, string> Create(ConsumerConfig config);
}