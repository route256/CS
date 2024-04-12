using System.Transactions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ozon.Route256.Practice.CustomerService.Exceptions;
using Ozon.Route256.Practice.CustomerService.GrpcServices.Extensions;
using Ozon.Route256.Practice.CustomerService.Repository;
using Ozon.Route256.Practice.CustomerService.Repository.Dto;

namespace Ozon.Route256.Practice.CustomerService.GrpcServices;

public sealed class CustomersService : Customers.CustomersBase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IAddressRepository  _addressRepository;

    public CustomersService(ICustomerRepository customerRepository, IAddressRepository addressRepository)
    {
        _customerRepository = customerRepository;
        _addressRepository  = addressRepository;
    }

    public override async Task<GetCustomersResponse> GetCustomers(Empty request, ServerCallContext context)
    {
        var customers = await _customerRepository.GetAll(context.CancellationToken);
        var defaultAddresses = (await _addressRepository.GetAll(context.CancellationToken))
           .ToDictionary(x => x.Id);

        var protoCustomers = ConvertToProtoTypes(customers, defaultAddresses);
        return new GetCustomersResponse
        {
            Customers = { protoCustomers }
        };
    }

    private static IEnumerable<Customer> ConvertToProtoTypes(IEnumerable<CustomerDto>    customers,
                                                             Dictionary<int, AddressDto> addresses)
    {
        foreach (var customer in customers)
        {
            if (!addresses.TryGetValue(customer.Id, out var currentAddress))
                throw new NotFoundException($"Default address for customer {customer.Id} not found");

            var addressesForCustomer = addresses.GetByKeys(customer.Addresses)
                                                .Select(x => x.ToProtoAddress());

            yield return customer.ToProtoCustomer(currentAddress.ToProtoAddress(), addressesForCustomer);
        }
    }

    public override async Task<Customer> GetCustomer(GetCustomerByIdRequest request, ServerCallContext context)
    {
        var customer = await _customerRepository.Find(request.Id, context.CancellationToken);
        if (customer is null)
            throw new NotFoundException($"Customer {request.Id} not found");

        var currentAddress = await _addressRepository.Find(customer.AddressID, context.CancellationToken);
        if (currentAddress is null)
            throw new NotFoundException($"Default address for customer {request.Id} not found");

        var addresses = await _addressRepository.FindMany(customer.Addresses, context.CancellationToken);

        return customer.ToProtoCustomer(currentAddress.ToProtoAddress(), addresses.Select(x => x.ToProtoAddress()));
    }

    public override async Task<GetCustomersForGeneratorResponse> GetCustomersForGenerator(GetCustomerByIdRequest request,
                                                                                          ServerCallContext      context)
    {
        var customer = await _customerRepository.Find(request.Id, context.CancellationToken);
        if (customer is null)
            throw new NotFoundException($"Customer {request.Id} not found");

        var currentAddress = await _addressRepository.Find(customer.AddressID, context.CancellationToken);
        if (currentAddress is null)
            throw new NotFoundException($"Default address for customer {request.Id} not found");

        return new GetCustomersForGeneratorResponse
        {
            Id      = request.Id,
            Address = currentAddress.ToProtoAddress()
        };
    }

    public override async Task<Empty> CreateCustomer(
        CreateCustomerRequest request,
        ServerCallContext context)
    {
        using (var ts = new TransactionScope(
                   TransactionScopeOption.Required,
                   new TransactionOptions
                   {
                       IsolationLevel = IsolationLevel.ReadCommitted,
                       Timeout = TimeSpan.FromSeconds(5)
                   },
                   TransactionScopeAsyncFlowOption.Enabled
                   ))
        {
        
            var address = new AddressDto(
                Id: default,
                Region: request.Customer.DefaultAddress.Region,
                City: request.Customer.DefaultAddress.City,
                Street: request.Customer.DefaultAddress.Street,
                Building: request.Customer.DefaultAddress.Building,
                Apartment: request.Customer.DefaultAddress.Apartment,
                Latitude: request.Customer.DefaultAddress.Latitude,
                Longitude: request.Customer.DefaultAddress.Longitude
            );
            
            var addressId = await _addressRepository.Create(address, context.CancellationToken);
            
            var customer = new CustomerDto(
                Id: request.Customer.Id,
                FirstName: request.Customer.FirstName,
                LastName: request.Customer.LastName,
                MobileNumber: request.Customer.MobileNumber,
                Email: request.Customer.Email,
                AddressID: addressId,
                Addresses: Array.Empty<int>());

            await _customerRepository.Create(
                customer,
                context.CancellationToken);
        
            ts.Complete();
        }

        return new Empty();
    }
}