namespace Ozon.Route256.Practice.OrdersGenerator.Models;

public record GoodModel(
    long Id,
    string Name,
    int Quantity,
    decimal Price,
    uint Weight);