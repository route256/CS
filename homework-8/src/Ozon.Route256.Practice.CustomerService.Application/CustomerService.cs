using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Ozon.Route256.Practice.CustomerService.Domain.Customer;
using Ozon.Route256.Practice.CustomerService.Domain.Exceptions;
using Ozon.Route256.Practice.CustomerService.Observation;

[assembly: InternalsVisibleTo("Ozon.Route256.Practice.CustomerService.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Ozon.Route256.Practice.CustomerService.Application;

internal sealed class CustomerService : ICustomerService
{
    private readonly ICustomerRepository      _customerRepository;
    private readonly IAddressRepository       _addressRepository;
    private readonly IUnitOfWork              _unitOfWork;
    private readonly ILogger<CustomerService> _logger;
    private readonly IBusinessMetrics         _businessMetrics;

    public CustomerService(
        ICustomerRepository      customerRepository,
        IAddressRepository       addressRepository,
        IUnitOfWork              unitOfWork,
        ILogger<CustomerService> logger,
        IBusinessMetrics         businessMetrics)
    {
        _customerRepository = customerRepository;
        _addressRepository  = addressRepository;
        _unitOfWork         = unitOfWork;
        _logger             = logger;
        _businessMetrics    = businessMetrics;
    }

    /// <inheritdoc />
    public async Task CreateCustomer(CustomerAggregate customer, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Creating customer {@Customer}", customer);

        await _unitOfWork.CreateTransactional(customer, cancellationToken);

        _businessMetrics.CustomerCreated(customer.DefaultAddress.Region);
        _logger.LogDebug("Customer {@Customer} created", customer);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<CustomerAggregate>> FindAllCustomers(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting all customers");

        var customers = await _customerRepository.GetAll(cancellationToken);
        var addresses = await _addressRepository.GetAll(cancellationToken);

        var result = customers.Select(customer => new CustomerAggregate(customer, addresses)).ToList();

        _logger.LogDebug("All customers loaded");

        return result;
    }

    /// <inheritdoc />
    public async Task<CustomerAggregate> GetCustomerById(int customerId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("GetCustomerById started {customerId}", customerId);

        var customer = await _customerRepository.Find(customerId, cancellationToken);

        if (customer == null)
        {
            _logger.LogDebug("GetCustomerById FAILED {customerId}", customerId);
            throw new CustomerNonFoundException();
        }

        var addresses = await _addressRepository.FindAllForCustomer(customerId, cancellationToken);

        var result = new CustomerAggregate(customer, addresses);

        _logger.LogDebug("GetCustomerById ended {customerId}", customerId);

        return result;
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