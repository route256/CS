using StackExchange.Redis;

namespace Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Redis;

public class RedisDatabaseFactory: IDisposable, IRedisDatabaseFactory
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisDatabaseFactory(
        string connectionString)
    {
        _connectionMultiplexer = ConnectionMultiplexer
            .Connect(connectionString);
    }

    public IDatabase GetDatabase() => 
        _connectionMultiplexer.GetDatabase();

    public IServer GetServer()
    {
        var endpoints = _connectionMultiplexer.GetEndPoints();
        return _connectionMultiplexer.GetServer(endpoints.First());
    }

    public void Dispose()
    {
        _connectionMultiplexer.Dispose();
    }
}
