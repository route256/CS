using Ozon.Route256.Practice.CustomerService.Repository.Dto;

namespace Ozon.Route256.Practice.CustomerService.Repository;

public interface IAddressRepository
{
    public Task<AddressDto?> Find(int id, CancellationToken token);

    public Task<AddressDto[]> FindMany(IEnumerable<int> ids, CancellationToken token);

    public Task<AddressDto[]> GetAll(CancellationToken token);
}