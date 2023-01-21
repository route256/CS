using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Ozon.WebApp.Entities;
using Ozon.WebApp.Services;

namespace Ozon.WebApp.Handlers;

internal interface IGetProductsHandler
{
    public class Request
    {
        [Display(Name = "client-name")]
        public string? ClientName { get; set; }
    }
    
    ICollection<Product> Handle(IGetProductsHandler.Request request);
}

[UsedImplicitly]
internal sealed class GetProductsHandler : IGetProductsHandler
{
    private readonly ProductStorage _productStorage;
    private readonly ClientStorage _clientStorage;

    public GetProductsHandler(
        ProductStorage productStorage,
        ClientStorage clientStorage)
    {
        _productStorage = productStorage;
        _clientStorage = clientStorage;
    }

    public ICollection<Product> Handle(IGetProductsHandler.Request request)
    {
        if (string.IsNullOrEmpty(request.ClientName))
            throw new ArgumentException(request.ClientName, nameof(request.ClientName));

        var client = _clientStorage.GetClientByNameOrDefault(request.ClientName);
        if (client == null)
            throw new Exception("Пожалуйста введите личные данные");
        
        ICollection<Product> products = _productStorage.GetAll().ToArray();
        
        return products
            .Where(x => x.Adult == client is { Age: > 18 })
            .OrderByDescending(x => RelevantSort(x, client))
            .ToArray();
    }
    
    private int RelevantSort(Product product, Client client)
    {
        int points = 0;

        if (product.IsSponsored)
            points += 5;

        if (product.Gender == client.Gender)
            points ++;

        switch (client.Age)
        {
            case < 15 when product.Age is Ages.Children:
                points++;
                break;
            
            case > 15 and < 21 when product.Age is Ages.Teenager:
                points++;
                break;
            
            case > 21 and < 55 when product.Age is Ages.Adult or Ages.Old:
                points++;
                break;

            case > 55 when product.Age is Ages.Old:
                points++;
                break;
        }

        if (product.Tags.Contains(client.Hobby))
            points++;
        
        return points;
    }
}