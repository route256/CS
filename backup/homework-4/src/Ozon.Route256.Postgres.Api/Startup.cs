using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Ozon.Route256.Postgres.Api.Services;
using Ozon.Route256.Postgres.Domain;
using Ozon.Route256.Postgres.Persistence;
using Ozon.Route256.Postgres.Persistence.Common;

namespace Ozon.Route256.Postgres.Api;

public sealed class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration) => _configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        NpgsqlConnection.GlobalTypeMapper.MapEnum<OrderState>("order_state");

        var connectionString = _configuration["ConnectionString"];

        services
            .AddGrpc()
            .Services
            .AddGrpcReflection()
            .AddFluentMigrator(
                connectionString,
                typeof(SqlMigration).Assembly);

        services.AddScoped<IOrderRepository, OrderRepository>(_ => new(connectionString));
    }

    public static void Configure(IApplicationBuilder app, IWebHostEnvironment env) =>
        app
            .UseRouting()
            .UseEndpoints(
                endpoints =>
                {
                    endpoints.MapGrpcService<OrderGrpcService>();
                    endpoints.MapGrpcReflectionService();
                });
}
