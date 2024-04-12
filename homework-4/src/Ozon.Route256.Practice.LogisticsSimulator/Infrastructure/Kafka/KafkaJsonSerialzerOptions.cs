using System.Text.Json;

namespace Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Kafka;

public class KafkaJsonSerializerOptions
{
    public static JsonSerializerOptions Default => 
        new();
}
