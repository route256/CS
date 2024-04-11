using Ozon.Route256.Practice.LogisticsSimulator.DateTimeProvider;
using Ozon.Route256.Practice.LogisticsSimulator.Handlers;
using Ozon.Route256.Practice.LogisticsSimulator.Infrastructure;
using Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Grpc;

namespace Ozon.Route256.Practice.LogisticsSimulator;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddDateTimeProvider()
            .AddHandlers()
            .AddInfrastructure();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(e =>
        {
            e.MapGrpcService<LogisticsSimulatorGrpcService>();
            e.MapGrpcReflectionService();
        });
    }
}