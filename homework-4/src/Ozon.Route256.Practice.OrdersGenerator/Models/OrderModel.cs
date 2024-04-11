namespace Ozon.Route256.Practice.OrdersGenerator.Models;

public record OrderModel(
    long Id,
    OrderSource Source,
    CustomerModel Customer,
    IEnumerable<GoodModel> Goods);