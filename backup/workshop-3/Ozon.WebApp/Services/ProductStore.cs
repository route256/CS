using Ozon.WebApp.Entities;

namespace Ozon.WebApp.Services;

public class ProductStore
{
    private readonly Dictionary<Guid, Product> _dictionary = new Dictionary<Guid, Product>();
    public void Add(Product product) => _dictionary.Add(product.Id, product);
}