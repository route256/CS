using Ozon.Route256.Practice.CustomerService.Domain.Customer;

namespace Ozon.Route256.Practice.CustomerService.Application;

/// <summary>
/// Represents IUnitOfWork class.
/// </summary>
internal interface IUnitOfWork
{
    Task CreateTransactional(CustomerAggregate customer, CancellationToken cancellationToken);
}
