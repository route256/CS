using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Ozon.Route256.Practice.CustomerService.IntegrationTests;

public sealed class CustomerServiceAppFactory : WebApplicationFactory<Startup>
{
    public Customers.CustomersClient CustomersClient { get; }

    public CustomerServiceAppFactory()
    {
        var hostBuilder = CreateHostBuilder();
        hostBuilder.ConfigureWebHost(builder => builder.UseTestServer());

        var client = CreateClient();
        var grpcChannel = GrpcChannel.ForAddress(client.BaseAddress!,
                                                 new GrpcChannelOptions
                                                 {
                                                     HttpClient = client,
                                                 });

        CustomersClient = new Customers.CustomersClient(grpcChannel);
    }

    /// <inheritdoc />
    protected override IHostBuilder CreateHostBuilder()
    {
        var memoryConfig = new MemoryConfigurationSource
        {
            InitialData = new[]
            {
                new KeyValuePair<string, string>("ROUTE256_SD_ADDRESS",    "http://localhost:6081"),
                new KeyValuePair<string, string>("ROUTE256_GRPC_PORT",     "5001"),
                new KeyValuePair<string, string>("ROUTE256_HTTP_PORT",     "5002"),
                new KeyValuePair<string, string>("DbOptions:ClusterName",  "customers-cluster"),
                new KeyValuePair<string, string>("DbOptions:DatabaseName", "customer-shard"),
                new KeyValuePair<string, string>("DbOptions:User",         "test"),
                new KeyValuePair<string, string>("DbOptions:Password",     "test"),
            }
        };

        var builder = Host.CreateDefaultBuilder()
                          .ConfigureAppConfiguration(cfg => cfg.Add(memoryConfig))
                          .ConfigureServices(services
                                                 => services.AddGrpcClient<Customers.CustomersClient>(o => o.Address = new Uri("https://localhost:5001")))
                          .ConfigureWebHostDefaults(hostBuilder => hostBuilder.UseStartup<Startup>());

        return builder;
    }
}