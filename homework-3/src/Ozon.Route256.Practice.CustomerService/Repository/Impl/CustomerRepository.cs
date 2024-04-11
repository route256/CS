using Ozon.Route256.Practice.CustomerService.Repository.Dto;

namespace Ozon.Route256.Practice.CustomerService.Repository.Impl;

public class CustomerRepository : ICustomerRepository
{
    private readonly InMemoryStorage _inMemoryStorage;

    public CustomerRepository(InMemoryStorage inMemoryStorage)
    {
        _inMemoryStorage = inMemoryStorage;
    }

    public Task<CustomerDto[]> GetAll(CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<CustomerDto[]>(token);

        return Task.FromResult(_inMemoryStorage.Customers.Values.ToArray());
    }

    public Task<CustomerDto?> Find(int id, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<CustomerDto?>(token);

        return _inMemoryStorage.Customers.TryGetValue(id, out var customer)
            ? Task.FromResult<CustomerDto?>(customer)
            : Task.FromResult<CustomerDto?>(null);
    }
}