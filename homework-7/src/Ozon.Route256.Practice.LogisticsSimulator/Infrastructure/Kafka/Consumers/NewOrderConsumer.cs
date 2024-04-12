using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Repositories;
using Ozon.Route256.Practice.LogisticsSimulator.Model;

namespace Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Kafka.Consumers;

public class NewOrderConsumer: BackgroundService
{
    private readonly ILogger<NewOrderConsumer> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IConsumerProvider _consumerProvider;
    private readonly IOptions<NewOrderConsumerConfig> _config;

    public NewOrderConsumer(
        ILogger<NewOrderConsumer> logger,
        IServiceScopeFactory serviceScopeFactory,
        IConsumerProvider consumerProvider,
        IOptions<NewOrderConsumerConfig> config)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _consumerProvider = consumerProvider;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (stoppingToken.IsCancellationRequested is false)
        {
            using var consumer = _consumerProvider
                .Create(_config.Value.Config);

            try
            {
                consumer.Subscribe("new_orders");

                await ConsumeCycle(
                    consumer,
                    stoppingToken);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Consumer error");
                
                try
                {
                    consumer.Unsubscribe();
                }
                catch
                {
                    // ignored
                }

                await Task.Delay(
                    TimeSpan.FromSeconds(1),
                    stoppingToken);
            }
        }
    }

    private async Task ConsumeCycle(
        IConsumer<string, string> consumer,
        CancellationToken ct)
    {
        while (ct.IsCancellationRequested is false)
        {
            var consumeResult = consumer.Consume(ct);
            await Handle(consumeResult, ct);
            consumer.Commit();
        }
    }

    private async Task Handle(
        ConsumeResult<string, string> consumeResult,
        CancellationToken ct)
    {
        var kafkaOrder = JsonSerializer.Deserialize<KafkaNewOrder>(
            consumeResult.Message.Value,
            KafkaJsonSerializerOptions.Default);

        if (kafkaOrder is null)
        {
            return;
        }
        
        using var scope = _serviceScopeFactory.CreateScope();
        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
        var order = kafkaOrder.ToDomain();
        await orderRepository
            .Insert(
                order,
                ct);
    }

    private record KafkaNewOrder(
        long OrderId)
    {
        public Order ToDomain()
        {
            return new Order(
                OrderId,
                OrderState.Created,
                DateTimeOffset.UtcNow);
        }
    }
}
