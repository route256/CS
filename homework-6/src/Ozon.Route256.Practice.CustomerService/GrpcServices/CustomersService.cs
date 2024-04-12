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
        var addresses = await _addressRepository.GetAll(context.CancellationToken);

        var protoCustomers = ConvertToProtoTypes(customers, addresses);
        return new GetCustomersResponse
        {
            Customers = { protoCustomers }
        };
    }

    private static IEnumerable<Customer> ConvertToProtoTypes(IEnumerable<CustomerDto>    customers,
                                                             AddressDto[] addresses)
    {
        foreach (var customer in customers)
        {

            var customerAddresses = addresses
                .Where(x => x.CustomerId == customer.Id);
            
            // if (!addresses.TryGetValue(customer.Id, out var currentAddress))
            //     throw new NotFoundException($"Default address for customer {customer.Id} not found");

            // var addressesForCustomer = addresses.GetByKeys(customer.Addresses)
            //                                     .Select(x => x.ToProtoAddress());

            // var addressesForCustomer = Array.Empty<Address>();

            yield return customer.ToProtoCustomer(customerAddresses.ToArray());
        }
    }

    public override async Task<Customer> GetCustomer(GetCustomerByIdRequest request, ServerCallContext context)
    {
        var customer = await _customerRepository.Find(request.Id, context.CancellationToken);
        if (customer is null)
            throw new NotFoundException($"Customer {request.Id} not found");

        // var currentAddress = await _addressRepository.FindDefaultForCustomer(customer.Id, context.CancellationToken);
        // if (currentAddress is null)
        //     throw new NotFoundException($"Default address for customer {request.Id} not found");

        var addresses = await _addressRepository.FindAllForCustomer(customer.Id, context.CancellationToken);

        return customer.ToProtoCustomer(addresses);
    }

    public override async Task<GetCustomersForGeneratorResponse> GetCustomersForGenerator(GetCustomerByIdRequest request,
                                                                                          ServerCallContext      context)
    {
        var customer = await _customerRepository.Find(request.Id, context.CancellationToken);
        if (customer is null)
            throw new NotFoundException($"Customer {request.Id} not found");

        var currentAddress = await _addressRepository.FindDefaultForCustomer(customer.Id, context.CancellationToken);
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
        
            var addresses = request.Customer.Addresses
                .Select(x => 
                    new AddressDto
                    {
                        CustomerId = request.Customer.Id,
                        IsDefault  = false,
                        Region     = request.Customer.DefaultAddress.Region,
                        City       = request.Customer.DefaultAddress.City,
                        Street     = request.Customer.DefaultAddress.Street,
                        Building   = request.Customer.DefaultAddress.Building,
                        Apartment  = request.Customer.DefaultAddress.Apartment,
                        Latitude   = request.Customer.DefaultAddress.Latitude,
                        Longitude  = request.Customer.DefaultAddress.Longitude
                    })
                .Concat(new []
                {
                new AddressDto
                {
                    CustomerId = request.Customer.Id,
                    IsDefault = true,
                    Region = request.Customer.DefaultAddress.Region,
                    City = request.Customer.DefaultAddress.City,
                    Street = request.Customer.DefaultAddress.Street,
                    Building = request.Customer.DefaultAddress.Building,
                    Apartment = request.Customer.DefaultAddress.Apartment,
                    Latitude = request.Customer.DefaultAddress.Latitude,
                    Longitude = request.Customer.DefaultAddress.Longitude
                }});
            
            await _addressRepository.Create(request.Customer.Id, addresses.ToArray(), context.CancellationToken);
            
            var customer = new CustomerDto
            {
                Id = request.Customer.Id,
                FirstName = request.Customer.FirstName,
                LastName = request.Customer.LastName,
                MobileNumber = request.Customer.MobileNumber,
                Email = request.Customer.Email
            };

            await _customerRepository.Create(
                customer,
                context.CancellationToken);
        
            // ts.Complete();
        // }

        return new Empty();
    }

    public override async Task<GetCustomerByLastNameResponse> GetCustomerByLastName(
        GetCustomerByLastNameRequest request,
        ServerCallContext context)
    {
        var customers = await _customerRepository.FindByLastName(
            request.LastName,
            context.CancellationToken);

        return new GetCustomerByLastNameResponse
        {
            Customers = { ConvertToProtoTypes(customers, Array.Empty<AddressDto>()) }
        };
    }
}