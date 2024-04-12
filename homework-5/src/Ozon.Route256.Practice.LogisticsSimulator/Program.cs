using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Ozon.Route256.Practice.LogisticsSimulator;

const int grpcPort = 80;

await Host
    .CreateDefaultBuilder(args)
    .ConfigureWebHost(w => 
        w.ConfigureKestrel(options =>
        {
            options.Listen(
                IPAddress.Any,
                grpcPort,
                listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
        })) 
    .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>()) 
    .Build()
    .RunAsync();