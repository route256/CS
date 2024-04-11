using System;
using Confluent.Kafka;

namespace Ozon.Route256.Postgres.Kafka;

/// <summary>
/// Class for special homework
/// </summary>
public class KafkaMessage<TKey, TValue> : Message<TKey, TValue>
{
    public void Handle() => throw new NotImplementedException();
}
