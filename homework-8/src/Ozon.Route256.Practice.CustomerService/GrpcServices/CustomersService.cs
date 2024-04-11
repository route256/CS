using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ozon.Route256.Practice.CustomerService.Application;
using Ozon.Route256.Practice.CustomerService.GrpcServices.Extensions;

namespace Ozon.Route256.Practice.CustomerService.GrpcServices;

public sealed class CustomersService : Customers.CustomersBase
{
    private readonly ICustomerService _customerService;

    public CustomersService(ICustomerService customerService)
        => _customerService = customerService;

    public override async Task<Empty> CreateCustomer(CreateCustomerRequest request, ServerCallContext context)
    {
        await _customerService.CreateCustomer(
            customer: request.Customer.ToDomainCustomer(),
            cancellationToken: context.CancellationToken);

        return new Empty();
    }

    public override async Task<GetCustomersResponse> GetCustomers(Empty request, ServerCallContext context)
    {
        var customers = await _customerService.FindAllCustomers(cancellationToken: context.CancellationToken);

        var protoCustomers = customers.Select(customer => customer.ToProtoCustomer());

        return new GetCustomersResponse
        {
            Customers = { protoCustomers }
        };
    }

    public override async Task<Customer> GetCustomer(GetCustomerByIdRequest request, ServerCallContext context)
    {
        var customer = await _customerService.GetCustomerById(request.Id, cancellationToken: context.CancellationToken);
        return customer.ToProtoCustomer();
    }

    public override async Task<GetCustomerByLastNameResponse> GetCustomerByLastName(
        GetCustomerByLastNameRequest request,
        ServerCallContext context)
    {
        var customers = await _customerService.FindCustomerByLastName(
            request.LastName, cancellationToken: context.CancellationToken);

        var protoCustomers = customers.Select(customer => customer.ToProtoCustomer());

        return new GetCustomerByLastNameResponse
        {
            Customers = { protoCustomers }
        };
    }

    public override async Task<GetCustomersForGeneratorResponse> GetCustomersForGenerator(
        GetCustomerByIdRequest request,
        ServerCallContext context)
    {
        var customers = await _customerService.GetCustomerForGenerator(request.Id, cancellationToken: context.CancellationToken);

        return new GetCustomersForGeneratorResponse
        {
            Id = request.Id,
            Address = customers.DefaultAddress.ToProtoAddress(),
        };
    }
}
