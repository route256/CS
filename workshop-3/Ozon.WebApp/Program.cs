namespace Ozon.WebApp;

public sealed class Program
{
    public static void Main(string[] args)
    {
        var hostBuilder = Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(x => x.UseStartup<Startup>());
        
        hostBuilder.Build().Run();
    }
}