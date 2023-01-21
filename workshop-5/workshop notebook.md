using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Google.Protobuf;
using Ozon.Route256.Postgres.Domain;
using Xunit;
using Xunit.Abstractions;

namespace Ozon.Route256.Postgres.IntegrationTests;

public sealed class KafkaTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    private readonly JsonSerializerOptions _jsonOptions;

    public KafkaTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _jsonOptions = new JsonSerializerOptions();
        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    private const string Broker = "localhost:9092";
    private const string Topic = "order_events";

    [Theory]
    [MemberData(nameof(GetEvents), 200)]
    public async Task ProduceMessage(OrderEvent orderEvent)
    {
        using var producer = new ProducerBuilder<long, OrderEvent>(
                new ProducerConfig
                {
                    BootstrapServers = Broker,
                })
            .SetValueSerializer(new JsonValueSerializer<OrderEvent>(_jsonOptions))
            .Build();

        await producer.ProduceAsync(
            Topic,
            new()
            {
                Headers = new()
                {
                    { "Producer", Encoding.Default.GetBytes(nameof(KafkaTests)) },
                    { "Machine", Encoding.Default.GetBytes(Environment.MachineName) }
                },
                Key = orderEvent.OrderId,
                Value = orderEvent
            });

        _testOutputHelper.WriteLine($"Produced message with key: {orderEvent.OrderId}");
        producer.Flush();
    }

    [Fact]
    public async Task ConsumeMessage()
    {
        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

        using var consumer = new ConsumerBuilder<long, OrderEvent>(
                new ConsumerConfig
                {
                    BootstrapServers = Broker,
                    GroupId = "user-notifications-service",

                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoCommit = true,
                    EnableAutoOffsetStore = false,
                    MaxPollIntervalMs = 1 * 60 * 1000 // 1 minute, default 5 min
                })
            .SetValueDeserializer(new JsonValueSerializer<OrderEvent>(_jsonOptions))
            .Build();

        consumer.Subscribe(Topic);

        while (consumer.Consume(cts.Token) is { } result)
        {
            cts.Token.ThrowIfCancellationRequested();

            _testOutputHelper.WriteLine($"Consumed key: {result.Message.Key} from partition: {result.Partition}");
            consumer.StoreOffset(result);
            await Task.Delay(1000, cts.Token);
        }

        consumer.Close();
    }

    public static IEnumerable<OrderEvent[]> GetEvents(int count)
    {
        const int maxOrders = 50;
        var enumValues = Enum.GetValues<OrderState>();
        var rnd = new Random();

        int i = 0;

        while (i < count)
            yield return new[]
            {
                new OrderEvent(
                    i++ % maxOrders,
                    rnd.Next(10000, 100000),
                    enumValues[rnd.Next(0, enumValues.Length - 1)],
                    DateTimeOffset.UtcNow - TimeSpan.FromSeconds(count - i))
            };
    }
}

public sealed record OrderEvent(long OrderId, long ClientId, OrderState State, DateTimeOffset TimeStamp);

internal sealed class JsonValueSerializer<T> : ISerializer<T>, IDeserializer<T>
{
    private JsonSerializerOptions _options;

    public JsonValueSerializer(JsonSerializerOptions options) => _options = options;

    public byte[] Serialize(T data, SerializationContext context) =>
        JsonSerializer.SerializeToUtf8Bytes(data);

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
            throw new ArgumentNullException($"Null data encountered deserializing {typeof(T).Name} value.");
        return JsonSerializer.Deserialize<T>(data) ?? throw new ArgumentNullException($"Cannot deserialize value of {typeof(T).Name}");
    }
}

internal sealed class ProtobufValueSerializer<T> : ISerializer<T>, IDeserializer<T> where T : IMessage<T>, new()
{
    private static readonly MessageParser<T> s_parser = new(() => new());

    public byte[] Serialize(T data, SerializationContext context) => data.ToByteArray();

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
            throw new ArgumentNullException($"Null data encountered deserializing {typeof(T).Name} value.");

        return s_parser.ParseFrom(data.ToArray());
    }
}
