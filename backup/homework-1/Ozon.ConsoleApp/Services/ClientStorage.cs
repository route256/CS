using System.Text.Json;
using Ozon.ConsoleApp.Entities;

namespace Ozon.ConsoleApp.Services;

public interface IClientStorage
{
    void Save(Client client);
}

public class ClientStorage : IClientStorage
{
    private const string FileName = "client.json";
    private const string StoragePath = $"{Program.StoragePath}/Clients";

    private string GetFilePath(string id)
    {
        return Path.Combine(StoragePath, $"{id}.{FileName}");
    }

    public void Save(Client client)
    {
        var path = GetFilePath(client.Name);
        var directory = Path.GetDirectoryName(path);

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory!);

        var json = JsonSerializer.Serialize(client, ClientGenerationContext.Custom.Client);
        File.WriteAllText(path, json);
    }

    public Client? GetClientByNameOrDefault(string requestClientName)
    {
        var path = GetFilePath(requestClientName);
        
        if (!File.Exists(path))
            return null;

        var json = File.ReadAllText(path);
        return TryGetFromJsonOrDefault(json);
    }
    
    private static Client? TryGetFromJsonOrDefault(string json)
    {
        try
        {
            var result = JsonSerializer.Deserialize<Client>(json);
            return result ?? null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}