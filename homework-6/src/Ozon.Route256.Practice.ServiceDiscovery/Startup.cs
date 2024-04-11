using Microsoft.Extensions.DependencyInjection.Extensions;
using Ozon.Route256.Practice.ServiceDiscovery.Configuration;
using Ozon.Route256.Practice.ServiceDiscovery.GrpcServices;

namespace Ozon.Route256.Practice.ServiceDiscovery;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<DbState>(
            o =>
            {
                var state = _configuration.GetValue<string>("ROUTE256_DB_STATE");
                var clusters = state.Split(";", StringSplitOptions.RemoveEmptyEntries);

                foreach (var cluster in clusters)
                {
                    var clusterSplitString = cluster.Split(":", StringSplitOptions.RemoveEmptyEntries);

                    var buckets = ConvertBucketToArray(clusterSplitString[1]);
                    var replica = new ReplicaInfo(clusterSplitString[2], Convert.ToInt32(clusterSplitString[3]), buckets);

                    if (o.Clusters.TryGetValue(clusterSplitString[0], out var hosts))
                    {
                        hosts.Add(replica);
                    }
                    else
                    {
                        o.Clusters.Add(
                            clusterSplitString[0],
                            new List<ReplicaInfo>
                            {
                                replica
                            });
                    }
                }
            });

        services.TryAddSingleton<IResourceStore, ResourceStore>();

        services.AddGrpc();
        services.AddGrpcReflection();
    }

    private static int[] ConvertBucketToArray(string s)
    {
        if (s == "0")
        {
            return Array.Empty<int>();
        }

        var bound = s.Split("-", StringSplitOptions.RemoveEmptyEntries);

        var low = Convert.ToInt32(bound[0]);
        var hi = Convert.ToInt32(bound[1]);
        var count = hi - low + 1;

        return Enumerable.Range(low, count).ToArray();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(e =>
        {
            e.MapGrpcService<SdServiceController>();
            e.MapGrpcReflectionService();
        });
    }
}
