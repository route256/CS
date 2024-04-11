# Ozon Route 256 — Kafka & Redis Examples

### Docker
* Запустить docker контейнеры (БД): `docker compose up -d`
* Остановить docker контейнеры (БД): `docker compose down`
* Остановить и почистить docker от данных: `docker compose down -v`
* Docker поломался: `docker system prune`

### Приложение

### Домашнее задание
Добавить к проекту (тот же самый, что и на прошлой неделе) Kafka и Redis.
* Добавить gRPC API, которое может менять статус у сущности «заказ».
* После фиксации изменения в БД необходимо отправлять сообщение об этом в топик Kafka. 
* Создать BackgroundHosted-сервис, который будет слушать Kafka и обновлять кэш в Redis-е по этому заказу
* API получения заказа должно первоначально искать данные в кэше, а затем уже — в БД.
* (Опциоанльно )Подумать, где тут есть слабые места и затеять обмен мнениями в Slack-е.

### Домашнее задание для упорных, бесстрашных людей с кучей свободного времени
* Создать channel-batch-консумер, который умеет читать много сообщений за раз и при этом предоставлять возможность работы как с Syste.Threading.Channel:

```csharp
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

    ...
}
```

```csharp
public class KafkaMessage<TKey, TValue> : Message<TKey, TValue>
{
    public void Handle() => ...
}
```


Example:
```csharp
var consumer = new ChannelConsumer<long, string>(
    new ConsumerConfig
    {
        EnableAutoCommit = true,
        EnableAutoOffsetStore = false
    },
    "topic_name",
    1000);

await foreach (var messageBuffer in consumer
                    .ReadAllAsync()
                    .Buffer(1000, TimeSpan.FromMilliseconds(10))
                    .WithCancellation())
foreach (var message in messageBuffer)
{
    // Do work
    message.Handle();
}
```
