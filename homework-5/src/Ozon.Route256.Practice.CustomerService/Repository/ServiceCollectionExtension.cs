using Ozon.Route256.Practice.CustomerService.Repository.Impl;

namespace Ozon.Route256.Practice.CustomerService.Repository;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryStorage>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        return services;
    }
}