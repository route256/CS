using Ozon.Route256.Practice.CustomerService.Domain.Customer;

namespace Ozon.Route256.Practice.CustomerService.Application;

public interface ICustomerRepository
{
    Task<Customer[]> GetAll(CancellationToken token);

    Task<Customer?> Find(int id, CancellationToken token);

    Task Create(Customer customer, CancellationToken token);

    Task<Customer[]> FindByLastName(string lastName, CancellationToken token);
}
