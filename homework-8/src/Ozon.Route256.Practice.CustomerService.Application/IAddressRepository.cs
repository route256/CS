using Ozon.Route256.Practice.CustomerService.Domain.Address;

namespace Ozon.Route256.Practice.CustomerService.Application;

public interface IAddressRepository
{
    public Task<Address?> FindDefaultForCustomer(int customerId, CancellationToken token);

    public Task<Address[]> FindAllForCustomer(int customerId, CancellationToken token);

    public Task<Address[]> GetAll(CancellationToken token);

    public Task Create(int customerId, Address[] address, CancellationToken token);
}
