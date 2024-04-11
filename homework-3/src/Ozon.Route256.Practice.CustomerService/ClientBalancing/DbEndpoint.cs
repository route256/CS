namespace Ozon.Route256.Practice.CustomerService.ClientBalancing;

public record DbEndpoint(
    string HostAndPort, 
    DbReplicaType DbReplica,
    int[] Buckets);