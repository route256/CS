using Ozon.Route256.Practice.CustomerService.Domain.Address;
using Ozon.Route256.Practice.CustomerService.Domain.Customer;

namespace Ozon.Route256.Practice.CustomerService.Application;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IAddressRepository _addressRepository;

    public UnitOfWork(ICustomerRepository customerRepository, IAddressRepository addressRepository)
    {
        _customerRepository = customerRepository;
        _addressRepository = addressRepository;
    }

    /// <inheritdoc />
    public async Task CreateTransactional(CustomerAggregate customer, CancellationToken cancellationToken)
    {
        // using (var ts = new TransactionScope(
        //            TransactionScopeOption.Required,
        //            new TransactionOptions
        //            {
        //                IsolationLevel = IsolationLevel.ReadCommitted,
        //                Timeout = TimeSpan.FromSeconds(5)
        //            },
        //            TransactionScopeAsyncFlowOption.Enabled
        //            ))
        // {
        await _customerRepository.Create(customer.Customer, cancellationToken);

        var allAddresses = new List<Address>
        {
            customer.DefaultAddress,
        };
        allAddresses.AddRange(customer.Addresses);

        await _addressRepository.Create(customer.Customer.Id, allAddresses.ToArray(), cancellationToken);

        // ts.Complete();
        // }
    }
}
