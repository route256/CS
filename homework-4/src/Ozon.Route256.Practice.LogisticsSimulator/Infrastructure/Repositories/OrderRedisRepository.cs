using System.Text.Json;
using Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Redis;
using Ozon.Route256.Practice.LogisticsSimulator.Model;
using StackExchange.Redis;
using Order = Ozon.Route256.Practice.LogisticsSimulator.Model.Order;

namespace Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Repositories;

public class OrderRedisRepository: IOrderRepository
{
    private readonly IDatabase _redisDatabase;
    private readonly IServer _redisServer;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new();

    public OrderRedisRepository(
        IRedisDatabaseFactory redisDatabaseFactory)
    {
        _redisDatabase = redisDatabaseFactory.GetDatabase();
        _redisServer = redisDatabaseFactory.GetServer();
    }

    private static string GetKey(long orderId) => 
        $"order:{orderId}";

    public async Task<Order?> Find(long orderId, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var value = await _redisDatabase
            .StringGetAsync(GetKey(orderId))
            .WaitAsync(token);

        return ToDomain(value);
    }

    public async Task Insert(Order order, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        var redisValue = ToRedisValue(order);
        await _redisDatabase
            .StringSetAsync(
                GetKey(order.OrderId),
                redisValue)
            .WaitAsync(token);
    }

    public async Task Update(Order order, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        var redisValue = ToRedisValue(order);
        var result = await _redisDatabase
            .StringSetAsync(
                GetKey(order.OrderId),
                redisValue,
                when: When.Exists)
            .WaitAsync(token);

        if (result is false)
        {
            throw new RepositoryException($"Order with id {order.OrderId} not found");
        }
    }

    public async Task<bool> IsExists(long orderId, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        var result = await _redisDatabase
            .KeyExistsAsync(
                GetKey(orderId))
            .WaitAsync(token);

        return result;
    }

    public async Task<ICollection<Order>> GetAll(CancellationToken token)
    {
        var keys = _redisServer
            .KeysAsync(pattern: "order:*");

        var redisValueTasks = await Get(
                keys)
            .ToListAsync(token);

        var redisValues = await Task.WhenAll(redisValueTasks);
        var orders = redisValues
            .Select(ToDomain)
            .Where(order => order is not null)
            .Select(order => order!.Value)
            .ToArray();

        return orders;

        async IAsyncEnumerable<Task<RedisValue>> Get(
            IAsyncEnumerable<RedisKey> redisKeys)
        {
            await foreach(var key in redisKeys.WithCancellation(token))
            {
                var getOrderTask = _redisDatabase
                    .StringGetAsync(key);

                yield return getOrderTask;
            }
        }
    }

    public async Task UpdateMany(IEnumerable<Order> orders, CancellationToken token)
    {
        var keyValues = orders
            .Select(order => new KeyValuePair<RedisKey, RedisValue>(
                GetKey(order.OrderId),
                ToRedisValue(order)))
            .ToArray();
        
        var result = await _redisDatabase
            .StringSetAsync(
                keyValues,
                when: When.Always)
            .WaitAsync(token);
    }

    private RedisValue ToRedisValue(Order order)
    {
        var redisOrder = new RedisOrder(
            order.OrderId,
            (int)order.OrderState,
            order.ChangedAt);

        return JsonSerializer.Serialize(
            redisOrder,
            _jsonSerializerOptions);
    }
    
    private Order? ToDomain(RedisValue redisValue)
    {
        if (string.IsNullOrWhiteSpace(redisValue))
        {
            return null;
        }
        
        var redisOrder = JsonSerializer.Deserialize<RedisOrder>(
            redisValue,
            _jsonSerializerOptions);

        return redisOrder?.ToDomain();
    }

    private record RedisOrder(
        long OrderId,
        int OrderState,
        DateTimeOffset ChangedAt)
    {
        public Order ToDomain()
        {
            return new Order(
                OrderId,
                (OrderState)OrderState,
                ChangedAt);
        }
    }
}
