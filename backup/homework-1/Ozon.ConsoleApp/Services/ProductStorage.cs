using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Ozon.ConsoleApp.Entities;

namespace Ozon.ConsoleApp.Services;

internal sealed class ProductStorage
{
    private const string StoragePath = $"{Program.StoragePath}/Products";

    public IEnumerable<Product> GetAll()
    {
        if (!Directory.Exists(StoragePath))
            yield break;

        foreach (var file in Directory.GetFiles(StoragePath))
        {
            var entity = GetEntity(file);
            if (entity != null)
                yield return entity;
        }
    }

    private static Product? GetEntity(string path)
    {
        if (!File.Exists(path))
            return null;

        var json = File.ReadAllText(path);
        return TryGetFromJsonOrDefault(json);
    }

    private static Product? TryGetFromJsonOrDefault(string json)
    {
        try
        {
            var result = JsonSerializer.Deserialize<Product>(json, JsonHelper.GetJsonSerializerOptions());
            return result ?? null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}