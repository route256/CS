using System;
using System.Net;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Ozon.Route256.Postgres.Api;

public static class Program
{
    public static void Main(string[] args) =>
        CreateHostBuilder(args)
            .Build()
            .RunWithMigrate(args);

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHost(
                builder =>
                {
                    builder.UseKestrel(o => o.Listen(IPAddress.Any, 5002, options => options.Protocols = HttpProtocols.Http2));
                    builder.UseStartup<Startup>();
                });

    private static void RunWithMigrate(this IHost host, string[] args)
    {
        if (args.Length > 0 && args[0].Equals("migrate", StringComparison.InvariantCultureIgnoreCase))
        {
            using var scope = host.Services.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

            runner.MigrateUp();
        }
        else
            host.Run();
    }
}
