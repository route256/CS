namespace Ozon.Route256.Practice.OrdersGenerator.Infrastructure.Kafka;

public interface IKafkaProducer
{
    Task SendMessage<TValue>(
        string topic,
        long key,
        TValue value,
        CancellationToken token);
}