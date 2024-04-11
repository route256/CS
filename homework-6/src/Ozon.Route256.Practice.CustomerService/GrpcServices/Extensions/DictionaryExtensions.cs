namespace Ozon.Route256.Practice.CustomerService.GrpcServices.Extensions;

public static class DictionaryExtensions
{
    public static IEnumerable<TValue> GetByKeys<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        IEnumerable<TKey>             keys)
        where TKey : notnull
    {
        foreach (var key in keys)
        {
            if (!dictionary.TryGetValue(key, out var value))
                throw new DictionaryExtensionException($"Value not found for key: {key}");

            yield return value;
        }
    }
}