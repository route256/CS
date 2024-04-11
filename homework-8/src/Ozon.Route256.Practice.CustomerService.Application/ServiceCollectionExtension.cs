using Microsoft.Extensions.DependencyInjection;

namespace Ozon.Route256.Practice.CustomerService.Application;

/// <summary>
/// Represents ServiceCollectionExtension class.
/// </summary>
public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
