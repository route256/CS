namespace Ozon.Route256.Practice.CustomerService.Infrastructure.ClientBalancing;

public record DbEndpoint(
    string HostAndPort, 
    DbReplicaType DbReplica,
    int[] Buckets);