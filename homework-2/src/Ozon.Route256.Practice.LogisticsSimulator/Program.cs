using Microsoft.AspNetCore.Server.Kestrel.Core;
using Ozon.Route256.Practice.LogisticsSimulator;

await Host
    .CreateDefaultBuilder(args)


    .ConfigureWebHost(w => 
        w.ConfigureKestrel(options =>
        {
            if(OperatingSystem.IsMacOS())
                options.ListenLocalhost(5238, o => o.Protocols = HttpProtocols.Http2); // Это что бы grpc работал на mac :(
        })) 
    .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>()) 
    .Build()
    .RunAsync();