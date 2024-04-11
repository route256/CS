using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeatherSimulator.Server.Extensions;
using WeatherSimulator.Server.GrpcServices;

namespace WeatherSimulator.Server;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSensorsHostedServices(Configuration);
        services.AddLogging();
        services.AddGrpc();
        services.AddGrpcReflection();
        services.AddSwaggerGen(opt => {
            
        });

        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(opt => {
                opt.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                opt.RoutePrefix = string.Empty;
            });
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<WeatherSimulatorService>();
            endpoints.MapGrpcReflectionService();
            endpoints.MapControllers();
        });
    }
}
