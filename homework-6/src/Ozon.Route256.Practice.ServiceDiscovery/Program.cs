using Microsoft.AspNetCore.Server.Kestrel.Core;
using Ozon.Route256.Practice.ServiceDiscovery;

await Host
    .CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(
        builder =>
        {
            builder.UseStartup<Startup>();
            builder.ConfigureAppConfiguration(configurationBuilder => configurationBuilder.AddEnvironmentVariables(prefix: "ROUTE256_"));
            builder.ConfigureKestrel(options => options.ConfigureEndpointDefaults(x => x.Protocols = HttpProtocols.Http2));
        })
    .Build()
    .RunAsync();