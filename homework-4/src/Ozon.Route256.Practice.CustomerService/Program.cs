using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Ozon.Route256.Practice.CustomerService;

await Host.CreateDefaultBuilder(args)
          .ConfigureWebHostDefaults(x => x.UseStartup<Startup>().ConfigureKestrel(options =>
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
           }))
          .Build()
          .RunAsync();