// See https://aka.ms/new-console-template for more information

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Google.Protobuf;
using GrpcEvent = Ozon.Route256.KafkaRedis.Grpc.OrderEvent;
using GrpcState = Ozon.Route256.KafkaRedis.Grpc.OrderState;
using Timestamp = Google.Protobuf.WellKnownTypes.Timestamp;

var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));

using var consumer = new ConsumerBuilder<long, GrpcEvent>(
        new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "user-notification-service",
            // AutoOffsetReset = AutoOffsetReset.Earliest,
            // MaxPollIntervalMs = 1 * 60 * 1000,
            // AutoCommitIntervalMs = 5 * 1000,
            EnableAutoCommit = true,
            EnableAutoOffsetStore = false
        })
    .SetValueDeserializer(new ProtobufSerializer<GrpcEvent>())
    .Build();

consumer.Subscribe("order_events");

while (consumer.Consume(cts.Token) is { } result)
{
    cts.Token.ThrowIfCancellationRequested();
    await Task.Delay(15000, cts.Token);
    consumer.StoreOffset(result);
    throw new Exception();
    // Console.WriteLine($"Consumed {result.Message.Value}, P: {result.Partition.Value}, O: {result.Offset.Value}");
}

consumer.Close();

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
