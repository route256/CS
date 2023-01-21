using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using AutoFixture;
using JetBrains.Annotations;
using Ozon.ConsoleApp.Entities;

namespace Ozon.ConsoleApp.Handlers;

[UsedImplicitly]
internal sealed class GenerateProductsHandler
{
    private const string StoragePath = $"{Program.StoragePath}/Products";
    private readonly string _fileExtension = "product.json";

    private readonly string[] _productColors =
    {
        "Серый",
        "Белый",
        "Желтый",
        "Красный",
        "Цвета морской волны",
        "Цвета мокрый асфальт"
    };

    private readonly string[] _productNames =
    {
        "Парик для питомца",
        "Накидка на телевизор",
        "Ковёр настенный",
        "Здоровенный ЯЗЬ",
        "Телепортатор",
        "Билет МММ"
    };

    private readonly Random _random = new();

    private readonly string[] _tags =
    {
        "Программирование",
        "Спорт",
        "Рисование",
        "Вязание",
        "Нарды",
        "Садоводство"
    };

    private string GetRandomName()
    {
        return $"{RandomValueFromArray(_productNames)}, {RandomValueFromArray(_productColors)}, {GetRandomVersion()}";
    }

    private string RandomValueFromArray(string[] array)
    {
        return array[_random.Next(0, array.Length-1)];
    }

    private string GetRandomVersion()
    {
        return $"V{_random.Next(1, 5)}.{_random.Next(1, 5)}";
    }

    private IEnumerable<string> GetRandomTags()
    {
        var count = _random.Next(1, _tags.Length);
        for (var i = 0; i < count; i++)
            yield return RandomValueFromArray(_tags);
    }

    public void Handle(Request request)
    {
        if (!request.Count.HasValue)
            throw new Exception("Количество не указано");

        var id = 0;

        RemoveAll();
        var fixture = new Fixture();
        fixture.Register(() => _random.Next(0, 2) == 0);

        var productsFixture = fixture
            .Build<Product>()
            .With(x => x.Name, GetRandomName)
            .With(x => x.Tags, () => GetRandomTags().Distinct().ToArray())
            .With(x => x.Id, () => ++id)
            .CreateMany(request.Count.Value);

        foreach (var product in productsFixture)
            Save(product);
    }

    public void Save(Product product)
    {
        var path = GetFilePath(product.Id);
        var directory = Path.GetDirectoryName(path);

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory!);

        var json = JsonSerializer.Serialize(product, JsonHelper.GetJsonSerializerOptions());
        File.WriteAllText(path, json);
    }

    public void RemoveAll()
    {
        if (!Directory.Exists(StoragePath))
            return;

        foreach (var file in Directory.GetFiles(StoragePath))
            File.Delete(file);
    }

    private string GetFilePath(int id)
    {
        return Path.Combine(StoragePath, $"{id}.{_fileExtension}");
    }

    public record Request([Display(Name = "count")] int? Count);
}