namespace Ozon.Route256.Practice.ServiceDiscovery.Configuration;

/// <summary>
/// Хранит данные о реплике БД 
/// </summary>
public record ReplicaInfo(string Host, int Port, int[] Buckets);