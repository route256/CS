using Ozon.Route256.Practice.OrdersGenerator.Generator;

namespace Ozon.Route256.Practice.OrdersGenerator.Infrastructure.Workers;

public class OrdersGeneratorWorker : BackgroundService
{
    private readonly Range _tasksDelayRange = new (500, 5000);
    private readonly Random _random = new ();
    
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OrdersGeneratorWorker> _logger;

    public OrdersGeneratorWorker(
        IServiceProvider serviceProvider,
        ILogger<OrdersGeneratorWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "OrdersGeneratorWorker running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            await DoWork(scope.ServiceProvider, stoppingToken);
            await Task.Delay(GetRandomDelay(), stoppingToken);
        }
    }

    private int GetRandomDelay()
    {
        return _random.Next(
            _tasksDelayRange.Start.Value,
            _tasksDelayRange.End.Value);
    }

    private async Task DoWork(
        IServiceProvider serviceProvider,
        CancellationToken token)
    {
        try
        {
            var generator = serviceProvider.GetRequiredService<IOrderGenerator>();
            await generator.GenerateOrder(token);
        }
        catch (Exception e)
        {
            _logger.LogInformation(
                "OrdersGeneratorWorker failed.", 
                e.Message);
        }
    }

    public override async Task StopAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "OrdersGeneratorWorker is stopping.");

        await base.StopAsync(stoppingToken);
    }
}