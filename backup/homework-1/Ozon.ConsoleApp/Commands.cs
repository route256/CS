using System.CommandLine;
using System.Text.Json;
using Ozon.ConsoleApp.Handlers;
using Ozon.ConsoleApp.Services;

namespace Ozon.ConsoleApp;

public class Commands
{
    public static IEnumerable<Command> CreateRootCommands(CancellationTokenSource cts)
    {
        yield return CreateExitCommand(cts);
        yield return CreateCreateClientCommand();
        yield return CreateUpdateProductsCommand();
        yield return CreateGetProductsCommand();
    }
    
    private static Command CreateUpdateProductsCommand()
    {
        var customerCommand = new Command("update-products");
        customerCommand.SetRequestHandler<GenerateProductsHandler.Request>(x =>
        {
            var handler = new GenerateProductsHandler();
            handler.Handle(x);
            Console.WriteLine("Success!");
        });
        
        return customerCommand;
    }
    
    private static Command CreateCreateClientCommand()
    {
        var customerCommand = new Command("client-add");

        customerCommand.SetRequestHandler<IAddNewClientHandler.Request>(x =>
        {
            var handler = new AddNewClientHandler(new ClientStorage());
            handler.Handle(x);
            Console.WriteLine("Success!");
        });
        
        return customerCommand;
    }
    
    private static Command CreateGetProductsCommand()
    {
        var customerCommand = new Command("get-products");

        customerCommand.SetRequestHandler<IGetProductsHandler.Request>(x =>
        {
            var handler = new GetProductsHandler(new ProductStorage(), new ClientStorage());
            var products = handler.Handle(x);
            WriteResponse(products);
        });
        
        return customerCommand;
    }

    private static Command CreateExitCommand(CancellationTokenSource cts)
    {
        var customerCommand = new Command("exit", "exit from program");
        customerCommand.SetHandler(x => cts.Cancel());
        return customerCommand;
    }
    
    private static void WriteResponse<TResponse>(TResponse response) 
        => Console.WriteLine(JsonSerializer.Serialize(response, JsonHelper.GetJsonSerializerOptions()));
}