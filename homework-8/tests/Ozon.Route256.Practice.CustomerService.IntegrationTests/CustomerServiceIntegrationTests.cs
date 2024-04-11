namespace Ozon.Route256.Practice.CustomerService.IntegrationTests;

public sealed class CustomerServiceIntegrationTests : IClassFixture<CustomerServiceAppFactory>
{
    private readonly CustomerServiceAppFactory _appFactory;

    public CustomerServiceIntegrationTests(CustomerServiceAppFactory appFactory)
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task GetCustomerTest()
    {
        var client = _appFactory.CustomersClient;

        var response = await client.GetCustomerAsync(new GetCustomerByIdRequest { Id = 102 });

        Assert.NotNull(response);
        Assert.Equal(102, response.Id);
    }
}