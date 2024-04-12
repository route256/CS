using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Ozon.Route256.Practice.OrdersGenerator.Configuration;

namespace Ozon.Route256.Practice.OrdersGenerator.Infrastructure.Kafka;

public class KafkaProducer: IKafkaProducer, IDisposable
{
    private readonly IProducer<long, string> _producer;

    private readonly ILogger<KafkaProducer> _logger;

    public KafkaProducer(
        IOptions<KafkaSettings> settings,
        ILogger<KafkaProducer> logger)
    {
        _logger = logger;
        var config = new ProducerConfig
        {
            BootstrapServers = settings.Value.Servers
        };

        _producer = new ProducerBuilder<long, string>(config).Build();
    }

    public async Task SendMessage<TValue>(
        string topic,
        long key,
        TValue value,
        CancellationToken token)
    {
        try
        {
            var payload = JsonConvert.SerializeObject(value);
            var message = new Message<long, string>
            {
                Key = key,
                Value = payload
            };
            await _producer.ProduceAsync(
                topic,
                message,
                token);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
}