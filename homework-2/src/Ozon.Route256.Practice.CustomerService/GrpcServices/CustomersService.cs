using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ozon.Route256.Practice.CustomerService.Exceptions;

namespace Ozon.Route256.Practice.CustomerService.GrpcServices;

public sealed class CustomersService: Customers.CustomersBase
{
    public override Task<GetCustomersResponse> GetCustomers(Empty request, ServerCallContext context)
    {
        return Task.FromResult(new GetCustomersResponse());
    }
    
    public override Task<Customer> GetCustomer(GetCustomerByIdRequest request, ServerCallContext context)
    {
        throw new NotFoundException($"Клиент с id={request.Id} не найден");
    }

    public override Task<GetCustomersForGeneratorResponse> GetCustomersForGenerator(GetCustomerByIdRequest request,
        ServerCallContext context)
    {
        throw new NotFoundException($"Клиент с id={request.Id} не найден");
    }
}