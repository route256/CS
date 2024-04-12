using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Kafka.Producers;

public interface IProducerProvider
{
    IProducer<string, string> Get();
}

public class ProducerProvider: IDisposable, IProducerProvider
{
    private readonly IOptions<KafkaProducerConfig> _config;

    private readonly IProducer<string, string> _producer;

    public ProducerProvider(
        IOptions<KafkaProducerConfig> config)
    {
        _config = config;
        _producer = new ProducerBuilder<string, string>(
                config.Value.Config)
            .Build();
    }

    public IProducer<string, string> Get() => 
        _producer;

    public void Dispose()
    {
        _producer.Dispose();
    }
}
