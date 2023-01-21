using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Ozon.Route256.Postgres.Kafka;

/// <summary>
/// Class for special homework
/// </summary>
public class ChannelConsumer<TKey, TValue> : ChannelReader<KafkaMessage<TKey, TValue>>, IDisposable
{
    public ChannelConsumer(
        ConsumerConfig config,
        string topic,
        int consumeSize,
        IDeserializer<TKey>? keyDeserializer = null,
        IDeserializer<TValue>? valueDeserializer = null)
    {
    }

    public void Dispose() => throw new NotImplementedException();

    public override bool TryRead(out KafkaMessage<TKey, TValue> item) => throw new NotImplementedException();

    public override ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();
}
