using Ozon.Route256.Practice.CustomerService.Repository.Dto;

namespace Ozon.Route256.Practice.CustomerService.Repository.Impl;

public class AddressRepository : IAddressRepository
{
    private readonly InMemoryStorage _inMemoryStorage;

    public AddressRepository(InMemoryStorage inMemoryStorage)
    {
        _inMemoryStorage = inMemoryStorage;
    }

    public Task<AddressDto?> Find(int id, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<AddressDto?>(token);

        return _inMemoryStorage.Addresses.TryGetValue(id, out var address)
            ? Task.FromResult<AddressDto?>(address)
            : Task.FromResult<AddressDto?>(null);
    }

    public Task<AddressDto[]> FindMany(IEnumerable<int> ids, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<AddressDto[]>(token);

        var addresses = FindDto(ids, token).ToArray();
        return Task.FromResult(addresses);
    }

    public Task<AddressDto[]> GetAll(CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<AddressDto[]>(token);

        return Task.FromResult(_inMemoryStorage.Addresses.Values.ToArray());
    }

    private IEnumerable<AddressDto> FindDto(IEnumerable<int> ids, CancellationToken token)
    {
        foreach (var id in ids)
        {
            token.ThrowIfCancellationRequested();

            if (_inMemoryStorage.Addresses.TryGetValue(id, out var address))
                yield return address;
        }
    }
}