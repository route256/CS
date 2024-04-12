using Ozon.Route256.Practice.CustomerService.Domain.Customer;
using Ozon.Route256.Practice.CustomerService.Domain.Exceptions;

namespace Ozon.Route256.Practice.CustomerService.Application;

internal sealed class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CustomerService(
        ICustomerRepository customerRepository,
        IAddressRepository addressRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _addressRepository = addressRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task CreateCustomer(CustomerAggregate customer, CancellationToken cancellationToken)
    {
        await _unitOfWork.CreateTransactional(customer, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<CustomerAggregate>> FindAllCustomers(CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.GetAll(cancellationToken);
        var addresses = await _addressRepository.GetAll(cancellationToken);

        return customers.Select(customer => new CustomerAggregate(customer, addresses)).ToList();
    }

    /// <inheritdoc />
    public async Task<CustomerAggregate> GetCustomerById(int customerId, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.Find(customerId, cancellationToken);

        if (customer == null)
        {
            throw new CustomerNonFoundException();
        }

        var addresses = await _addressRepository.FindAllForCustomer(customerId, cancellationToken);

        return new CustomerAggregate(customer, addresses);
    }

    /// <inheritdoc />
    public async Task<CustomerAggregate> GetCustomerForGenerator(int customerId, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.Find(customerId, cancellationToken);

        if (customer == null)
        {
            throw new CustomerNonFoundException();
        }

        var addresses = await _addressRepository.FindAllForCustomer(customerId, cancellationToken);

        return new CustomerAggregate(customer, addresses);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<CustomerAggregate>> FindCustomerByLastName(string lastName, CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.FindByLastName(lastName, cancellationToken);
        
        var result = new List<CustomerAggregate>();

        foreach (var customer in customers)
        {
            var addresses = await _addressRepository.FindAllForCustomer(customer.Id, cancellationToken);
            result.Add(new CustomerAggregate(customer, addresses));
        }

        return result;
    }
}
