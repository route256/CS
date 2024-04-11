// See https://aka.ms/new-console-template for more information

using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using Confluent.Kafka;
using Google.Protobuf;
using GrpcEvent = Ozon.Route256.KafkaRedis.Grpc.OrderEvent;
using GrpcState = Ozon.Route256.KafkaRedis.Grpc.OrderState;
using Timestamp = Google.Protobuf.WellKnownTypes.Timestamp;

const string broker = "localhost:9092";
const string topic = "order_events";

var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

using var producer = new ProducerBuilder<long, GrpcEvent>(
        new ProducerConfig
        {
            BootstrapServers = broker,
            Acks = Acks.All
        })
    .SetValueSerializer(new ProtobufSerializer<GrpcEvent>())
    .Build();

var events = new GrpcEvent[]
{
    new()
    {
        OrderId = 10,
        ClientId = 448348,
        OrderState = GrpcState.Created,
        Timestamp = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow - TimeSpan.FromMinutes(1))
    }
};

// var events = new OrderEvent[]
// {
//     new OrderEvent(10, 448348, OrderState.Created, DateTimeOffset.UtcNow - TimeSpan.FromMinutes(1)),
//     new OrderEvent(10, 448348, OrderState.Changed, DateTimeOffset.UtcNow),
//
//     new OrderEvent(11, 448346, OrderState.Created, DateTimeOffset.UtcNow - TimeSpan.FromMinutes(2)),
//     new OrderEvent(11, 448346, OrderState.Changed, DateTimeOffset.UtcNow - TimeSpan.FromMinutes(1)),
//     new OrderEvent(11, 448346, OrderState.Completed, DateTimeOffset.UtcNow),
//
//     new OrderEvent(12, 448346, OrderState.Created, DateTimeOffset.UtcNow - TimeSpan.FromMinutes(2)),
//     new OrderEvent(12, 448346, OrderState.Changed, DateTimeOffset.UtcNow - TimeSpan.FromMinutes(1)),
//     new OrderEvent(12, 448346, OrderState.Completed, DateTimeOffset.UtcNow)
// };

foreach (var orderEvent in events)
{
    await producer.ProduceAsync(
        topic,
        new Message<long, GrpcEvent>
        {
            Headers = new()
            {
                { "Producer", Encoding.Default.GetBytes("KafkaTestProducer") },
                { "Machine", Encoding.Default.GetBytes(Environment.MachineName) }
            },
            Key = orderEvent.OrderId,
            Value = orderEvent
        },
        cts.Token);
    Console.WriteLine($"Produced: {orderEvent}");
}

producer.Flush();

public enum OrderState
{
    Created,
    Changed,
    Completed
}

public sealed record OrderEvent(long OrderId, long ClientId, OrderState State, DateTimeOffset Timestamp);

public sealed class JsonValueSerializer<T> : ISerializer<T>, IDeserializer<T>
{
    private static readonly JsonSerializerOptions _serializerOptions;

    static JsonValueSerializer()
    {
        _serializerOptions = new JsonSerializerOptions();
        _serializerOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public byte[] Serialize(T data, SerializationContext context) => JsonSerializer.SerializeToUtf8Bytes(data, _serializerOptions);

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
            throw new ArgumentNullException(nameof(data), "Null data encountered");

        return JsonSerializer.Deserialize<T>(data, _serializerOptions) ??
               throw new ArgumentNullException(nameof(data), "Null data encountered");
    }
}

public sealed class ProtobufSerializer<T> : ISerializer<T>, IDeserializer<T>
    where T : IMessage<T>, new()
{
    private static readonly MessageParser<T> s_parser = new(() => new());

    public byte[] Serialize(T data, SerializationContext context) => data.ToByteArray();

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
            throw new ArgumentNullException(nameof(data), "Null data encountered");

        return s_parser.ParseFrom(data);
    }
}
