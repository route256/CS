namespace Ozon.Route256.Practice.OrdersGenerator.Providers.Goods;

public record GoodDto(
    long Id,
    string Name,
    decimal Price,
    uint Weight);