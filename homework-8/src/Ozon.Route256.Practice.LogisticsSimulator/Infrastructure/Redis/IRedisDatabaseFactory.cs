using StackExchange.Redis;

namespace Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Redis;

public interface IRedisDatabaseFactory
{
    IDatabase GetDatabase();
    IServer GetServer();
}