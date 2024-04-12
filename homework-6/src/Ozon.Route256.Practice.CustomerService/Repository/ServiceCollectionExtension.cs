using Ozon.Route256.Practice.CustomerService.Repository.Impl;

namespace Ozon.Route256.Practice.CustomerService.Repository;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAddressRepository, ShardAddressRepository>();
        services.AddScoped<ICustomerRepository, ShardCustomerRepository>();

        return services;
    }
}