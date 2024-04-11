using Confluent.Kafka;

namespace Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Kafka.Consumers;

public class ConsumerProvider : IConsumerProvider
{
    private readonly ILogger<ConsumerProvider> _logger;

    public ConsumerProvider(
        ILogger<ConsumerProvider> logger)
    {
        _logger = logger;
    }

    public IConsumer<string, string> Create(ConsumerConfig config)
    {
        return new ConsumerBuilder<string, string>(
                config)
            .SetPartitionsAssignedHandler(
                (consumer, topicPartitions) =>
                    _logger.LogInformation(
                        "Partition assigned: {TopicPartitions}",
                        string.Join(
                            Environment.NewLine,
                            topicPartitions
                                .Select(part => $"{part.Topic}: {part.Partition.ToString()}"))))
            .Build();
    }
}
