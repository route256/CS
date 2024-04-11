using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherSimulator.Server.Configurations;
using WeatherSimulator.Server.Services;
using WeatherSimulator.Server.Services.Abstractions;
using WeatherSimulator.Server.Services.Background;
using WeatherSimulator.Server.Storages;
using WeatherSimulator.Server.Storages.Abstractions;

namespace WeatherSimulator.Server.Extensions;

public static class ServiceCollectionsExtensions
{
    public static IServiceCollection AddSensorsHostedServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<WeatherServerConfiguration>(configuration.GetSection(nameof(WeatherServerConfiguration)));
        services.AddHostedService<SensorPoolingService>();
        services.AddSingleton<IMeasureSubscriptionStore, MeasureSubscriptionStore>();
        services.AddSingleton<IMeasureService, MeasureService>();
        
        return services;
    }
}
