using Ozon.Route256.Practice.LogisticsSimulator.Handlers.OrdersRandomUpdate;
using Ozon.Route256.Practice.LogisticsSimulator.Handlers.ResultTypes;

namespace Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.BackgroundJobs;

public class UpdateOrderJob: BackgroundService
{
    private const int TasksDelay = 5000; // TODO: Потом вынести в динамическую настройку
    
    private readonly ILogger<UpdateOrderJob> _logger;
    private readonly IServiceProvider _services;

    public UpdateOrderJob(IServiceProvider services, 
        ILogger<UpdateOrderJob> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Consume Scoped Service Hosted Service running.");

        var random = new Random();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _services.CreateScope();
            var eventId = random.Next();
            await DoWork(eventId, scope.ServiceProvider, stoppingToken);
            await Task.Delay(TasksDelay, stoppingToken);
        }
    }

    private async Task DoWork(int eventId, IServiceProvider serviceProvider, CancellationToken stoppingToken)
    {
        _logger.LogInformation(eventId, "UpdateOrderJob start working");
        var handler = serviceProvider.GetRequiredService<IOrdersRandomUpdateHandler>();
        var result = await handler.Handle(stoppingToken);
        CheckResult(eventId, result);
    }
    

    private void CheckResult(int eventId, HandlerResult result)
    {
        if(result.Success)
            _logger.LogInformation(eventId, "UpdateOrderJob finished successfully");
        else
            _logger.LogWarning(eventId, result.Error, "UpdateOrderJob finished with errors");
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Consume Scoped Service Hosted Service is stopping.");

        await base.StopAsync(stoppingToken);
    }
}