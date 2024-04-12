using System.Collections.Concurrent;
using Ozon.Route256.Practice.LogisticsSimulator.Model;

namespace Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Repositories;

public class OrderInMemoryRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<long, Order> _orders = new();
    
    private readonly Task _completedTask = Task.CompletedTask;

    public Task<Order?> Find(long orderId, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<Order?>(token);

        Order? result = _orders.TryGetValue(orderId, out var order) ? order : null;
        
        return Task.FromResult(result).WaitAsync(token);
    }

    public Task Insert(Order order, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled(token);
        
        if (_orders.ContainsKey(order.OrderId))
            throw new RepositoryException($"Order with id {order.OrderId} already exists");

        _orders[order.OrderId] = order;

        return _completedTask.WaitAsync(token);
    }

    public Task Update(Order order, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled(token);
        
        if(!_orders.ContainsKey(order.OrderId))
            throw new RepositoryException($"Order with id {order.OrderId} not found");
        
        _orders[order.OrderId] = order;
        
        return _completedTask.WaitAsync(token);
    }

    public Task<bool> IsExists(long orderId, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<bool>(token);

        var contains = _orders.ContainsKey(orderId);
        return Task.FromResult(contains).WaitAsync(token);
    }
    
    public Task<ICollection<Order>> GetAll(CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<ICollection<Order>>(token);

        var orders = _orders.Values;
        return Task.FromResult(orders).WaitAsync(token);
    }

    public Task UpdateMany(IEnumerable<Order> orders, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<bool>(token);

        foreach (var order in orders)
        {
            if(!_orders.ContainsKey(order.OrderId))
                throw new RepositoryException($"Order with id {order.OrderId} not found");
        
            _orders[order.OrderId] = order;
        }
        
        return _completedTask.WaitAsync(token);
    }
}