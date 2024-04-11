namespace Ozon.Route256.Practice.OrdersGenerator.Providers.Customers;

public interface ICustomerProvider
{
    Task<CustomerDto> GetRandomCustomer();
}