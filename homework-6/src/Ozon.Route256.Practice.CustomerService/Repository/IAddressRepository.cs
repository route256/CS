using Ozon.Route256.Practice.CustomerService.Repository.Dto;

namespace Ozon.Route256.Practice.CustomerService.Repository;

public interface IAddressRepository
{
    public Task<AddressDto?> FindDefaultForCustomer(int customerId, CancellationToken token);

    public Task<AddressDto[]> FindAllForCustomer(int customerId, CancellationToken token);

    public Task<AddressDto[]> GetAll(CancellationToken token);
    
    public Task Create(int customerId, AddressDto[] address, CancellationToken token);
}