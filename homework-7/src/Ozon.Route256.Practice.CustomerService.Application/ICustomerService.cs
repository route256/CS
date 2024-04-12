using Ozon.Route256.Practice.CustomerService.Domain.Customer;

namespace Ozon.Route256.Practice.CustomerService.Application;

/// <summary>
/// Основной сервис работы с клиентом
/// </summary>
public interface ICustomerService
{
    Task CreateCustomer(CustomerAggregate customer, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<CustomerAggregate>> FindAllCustomers(CancellationToken cancellationToken);

    Task<CustomerAggregate> GetCustomerById(int customerId, CancellationToken cancellationToken);

    Task<CustomerAggregate> GetCustomerForGenerator(int customerId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<CustomerAggregate>> FindCustomerByLastName(string lastName, CancellationToken cancellationToken);
}
