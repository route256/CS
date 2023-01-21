using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Ozon.WebApp;

namespace IntegrationTests;

public class UnitTest1 : IClassFixture<AppFactory>
{
    private readonly AppFactory _factory;

    public UnitTest1(AppFactory factory) => _factory = factory;

    [Fact]
    public void Test1()
    {
    }
}

public class AppFactory : WebApplicationFactory<Startup>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(x =>
        {

        });
        
        return base.CreateHost(builder);
    }
}