using Ozon.WebApp.Handlers;
using Ozon.WebApp.Services;

namespace Ozon.WebApp;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration) => _configuration = configuration;

    public void ConfigureServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddControllers();
        serviceCollection.AddSwaggerGen();
        serviceCollection.AddGrpc()
            .AddServiceOptions<ProductGrpcService>(x => x.Interceptors.Add<ServerLoggerInterceptor>());
        
        serviceCollection.AddSingleton<ProductStore>();
        serviceCollection.AddSingleton<StreamingService>();
        serviceCollection.AddScoped<IClientStorage, ClientStorage>();
        serviceCollection.AddScoped<IAddNewClientHandler.RequestValidator>();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseSwagger();
        app.UseSwaggerUI();
        
        app.UseEndpoints(x =>
        {
            x.MapGrpcService<ProductGrpcService>();
            x.MapControllers();
            x.MapGet("/", async context => await context.Response.WriteAsync("Hello Word"));
        });

    }
}