using Ozon.Route256.Practice.LogisticsSimulator.Model;

namespace Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Repositories;

public interface IOrderRepository
{
    Task<Order?> Find(long orderId, CancellationToken token);
    Task Insert(Order order, CancellationToken token);
    Task Update(Order order, CancellationToken token);
    Task<bool> IsExists(long orderId, CancellationToken token);
    Task<ICollection<Order>> GetAll(CancellationToken token);
    Task UpdateMany(IEnumerable<Order> orders, CancellationToken token);
}