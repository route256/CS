using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Ozon.Route256.Practice.CustomerService.Application;
using Ozon.Route256.Practice.CustomerService.GrpcServices;
using Ozon.Route256.Practice.CustomerService.Infrastructure;
using Ozon.Route256.Practice.CustomerService.Infrastructure.ClientBalancing;
using Ozon.Route256.Practice.CustomerService.Infrastructure.Dal.Common;
using Ozon.Route256.Practice.CustomerService.Infrastructure.Dal.Common.Shard;
using Ozon.Route256.Practice.CustomerService.Infrastructure.Logging;
using Ozon.Route256.Practice.CustomerService.Infrastructure.Metrics;
using Ozon.Route256.Practice.CustomerService.Infrastructure.Tracing;
using Ozon.Route256.Practice.CustomerService.Observation;
using Prometheus;
using Serilog;

namespace Ozon.Route256.Practice.CustomerService;

public sealed class Startup
{
    private readonly IConfiguration      _configuration;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IHostEnvironment    _hostEnvironment;

    public Startup(
        IConfiguration      configuration,
        IWebHostEnvironment webHostEnvironment,
        IHostEnvironment    hostEnvironment)
    {
        _configuration      = configuration;
        _webHostEnvironment = webHostEnvironment;
        _hostEnvironment    = hostEnvironment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
                    .Enrich.WithMemoryUsage()
                    .ReadFrom.Configuration(_configuration)
                    .CreateLogger();

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddSerilog();

        services.AddSingleton<ICustomerActivitySource, CustomerActivitySource>();
        services.AddSingleton<IGrpcMetrics, GrpcMetrics>();
        services.AddSingleton<IBusinessMetrics, BusinessMetrics>();
        services.AddSingleton<IDbStore, DbStore>();
        services.AddGrpc(x =>
        {
            x.Interceptors.Add<LoggerInterceptor>();
            x.Interceptors.Add<TraceInterceptor>();
            x.Interceptors.Add<MetricsInterceptor>();
        });

        services.AddOpenTelemetry()
                .WithTracing(x =>
                 {
                     x.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(nameof(CustomersService)));
                     x.AddAspNetCoreInstrumentation();
                     x.AddNpgsql();
                     x.AddSource(CustomerActivitySource.ActivityName);
                     x.AddConsoleExporter();
                     x.AddOtlpExporter();
                 });

        // var factory = new StaticResolverFactory(address => new[]
        // {
        //     new BalancerAddress("service-discovery-1", 80),
        //     new BalancerAddress("service-discovery-2", 80)
        // });
        //
        // services.AddSingleton<ResolverFactory>(factory);

        services.AddGrpcClient<SdService.SdServiceClient>(options =>
        {
            var url = _configuration.GetValue<string>("ROUTE256_SD_ADDRESS");
            if (string.IsNullOrEmpty(url))
                throw new Exception("ROUTE256_SD_ADDRESS variable is empty");

            options.Address = new Uri(url);
            //options.Address = new Uri("static:///sd-service");
        });
        // .ConfigureChannel(x =>
        // {
        //     x.Credentials = ChannelCredentials.Insecure;
        //     x.ServiceConfig = new ServiceConfig()
        //     {
        //         LoadBalancingConfigs = { new LoadBalancingConfig("round_robin") }
        //     };
        // });

        services.AddHostedService<ServiceDiscoveryConsumerHostedService>();
        services.AddGrpcReflection();
        services.AddApplication();
        services.AddInfrastructure();

        services.AddControllers();
        services.AddEndpointsApiExplorer();

        services.AddGrpcSwagger();

        var connectionString = _configuration.GetConnectionString("CustomerDb");

        services.AddSingleton<IPostgresConnectionFactory>(_ => new PostgresConnectionFactory(connectionString));

        // services.AddFluentMigratorCore()
        //     .ConfigureRunner(
        //         builder => builder
        //             .AddPostgres()
        //             .ScanIn(typeof(SqlMigration).Assembly)
        //             .For.Migrations())
        //     .AddOptions<ProcessorOptions>()
        //     .Configure(
        //         options =>
        //         {
        //             options.ConnectionString = connectionString;
        //             options.Timeout          = TimeSpan.FromSeconds(30);
        //         });

        PostgresMapping.MapCompositeTypes();

        services.Configure<DbOptions>(_configuration.GetSection(nameof(DbOptions)));
        services.AddSingleton<IShardConnectionFactory, ShardConnectionFactory>();
        services.AddSingleton<IShardingRule<long>, LongShardingRule>();
        services.AddSingleton<IShardingRule<string>, StringShardingRule>();
        services.AddSingleton<IShardMigrator, ShardMigrator>();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseRouting();
        app.UseEndpoints(x =>
        {
            x.MapGet("", () => "Hello World!");
            x.MapGrpcService<CustomersService>();
            x.MapGrpcReflectionService();
            x.MapMetrics();
        });
    }
}