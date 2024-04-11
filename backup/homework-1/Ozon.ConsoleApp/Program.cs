using System.CommandLine;

namespace Ozon.ConsoleApp;

public sealed class Program
{
    public const string StoragePath = "Storage";
    private static readonly CancellationTokenSource Cts = new();
    private static readonly string SessionArgument = "--session";

    static Program()
    {
    }

    public static async Task Main(string[]? args)
    {
        if (args?.FirstOrDefault() == SessionArgument)
            await RunSession(args.Skip(1).ToArray());
        else
            await InvokeSingle(args);
    }

    private static async Task RunSession(string[]? args)
    {
        Console.WriteLine("Application started...");
        var rootCommand = CreateRootCommand(Cts);

        while (true)
        {
            if (args?.Any() ?? false) 
                await rootCommand.InvokeAsync(args);

            if (Cts.Token.IsCancellationRequested)
                return;

            args = Console.ReadLine()?.Split(' ');
        }
    }
    
    private static async Task InvokeSingle(string[]? args)
    {
        var rootCommand = CreateRootCommand(Cts);
        await rootCommand.InvokeAsync(args ?? Array.Empty<string>());
    }
    
    private static RootCommand CreateRootCommand(CancellationTokenSource cts)
    {
        var rootCommand = new RootCommand();
        foreach (var command in Commands.CreateRootCommands(cts))
            rootCommand.Add(command);

        return rootCommand;
    }
}