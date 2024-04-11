using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Practice.CustomerService.Application;
using Ozon.Route256.Practice.CustomerService.Infrastructure.Repository.Impl;

namespace Ozon.Route256.Practice.CustomerService.Infrastructure;

/// <summary>
/// Represents ServiceCollectionExtension class.
/// </summary>
public static class ServiceCollectionExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IAddressRepository, ShardAddressRepository>();
        services.AddScoped<ICustomerRepository, ShardCustomerRepository>();

        return services;
    }
}
