using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Ozon.Route256.Practice.LogisticsSimulator.Model;

namespace Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Kafka.Producers;

public class OrderEventProducer
{
    private readonly IProducerProvider _producerProvider;
    private readonly IOptions<KafkaProducerConfig> _config;

    public OrderEventProducer(
        IProducerProvider producerProvider,
        IOptions<KafkaProducerConfig> config)
    {
        _producerProvider = producerProvider;
        _config = config;
    }

    public async Task Produce(
        Order order,
        CancellationToken ct)
    {
        var producer = _producerProvider.Get();
        var kafkaMessage = ToKafka(order);
        await producer.ProduceAsync(
            _config.Value.OrderEventTopic,
            kafkaMessage,
            ct);
    }

    private static Message<string, string> ToKafka(Order order)
    {
        var kafkaOrder = new KafkaOrderUpdate(
            order.OrderId,
            order.OrderState,
            order.ChangedAt);

        var value = JsonSerializer.Serialize(
            kafkaOrder,
            KafkaJsonSerializerOptions.Default);

        return new Message<string, string>
        {
            Key = order.OrderId.ToString(),
            Value = value
        };
    }

    private record KafkaOrderUpdate(
        long OrderId,
        OrderState OrderState,
        DateTimeOffset ChangedAt);
}
