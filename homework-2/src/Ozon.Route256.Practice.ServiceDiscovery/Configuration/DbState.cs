namespace Ozon.Route256.Practice.ServiceDiscovery.Configuration;

/// <summary>
/// Хранит текущее состояние хостов баз данных 
/// </summary>
public record DbState
{
    public DbState()
        : this(new Dictionary<string, List<ReplicaInfo>>(0))
    {
    }

    /// <summary>
    /// Хранит текущее состояние хостов баз данных 
    /// </summary>
    /// <param name="clusters">Список баз данных с их хостами</param>
    public DbState(Dictionary<string, List<ReplicaInfo>> clusters)
    {
        this.Clusters = clusters;
    }

    /// <summary>Список баз данных с их хостами</summary>
    public Dictionary<string, List<ReplicaInfo>> Clusters { get; init; }
    
    public void Deconstruct(out Dictionary<string, List<ReplicaInfo>> DbHosts)
    {
        DbHosts = this.Clusters;
    }
}