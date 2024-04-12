using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Ozon.Route256.Practice.CustomerService.Infrastructure.Dal.Common.Shard;
using Serilog;

namespace Ozon.Route256.Practice.CustomerService;

public static class Program
{
    public static async Task Main(string[] args)
    {
        await CreateHostBuilder(args)
            .UseSerilog()
            .Build()
            .RunWithMigrate(args);
    }

    private static IHostBuilder CreateHostBuilder(
        string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(
                x => x.UseStartup<Startup>()
                    .ConfigureKestrel(
                        options =>
                        {
                            var grpcPort = int.Parse(Environment.GetEnvironmentVariable("ROUTE256_GRPC_PORT")!);
                            var httpPort = int.Parse(Environment.GetEnvironmentVariable("ROUTE256_HTTP_PORT")!);

                            options.Listen(
                                IPAddress.Any,
                                grpcPort,
                                listenOptions => listenOptions.Protocols = HttpProtocols.Http2);

                            options.Listen(
                                IPAddress.Any,
                                httpPort,
                                listenOptions => listenOptions.Protocols = HttpProtocols.Http1);
                        }));

    static async Task RunWithMigrate(
        this IHost host,
        string[] args)
    {
        var needMigration = args.Length > 0 && args[0].Equals("migrate");
        if (needMigration)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
            using var scope = host.Services.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IShardMigrator>();
            await runner.Migrate(cts.Token);
        }
        else
        {
            await host.RunAsync();
        }
    }
    
}