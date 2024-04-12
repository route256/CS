using Bogus;

namespace Ozon.Route256.Practice.OrdersGenerator.Providers.Goods;

public class GoodsProvider: IGoodsProvider
{
    private static readonly Faker Faker = new();
    private static readonly int GoodsCount = 50;
    private static readonly List<GoodDto> Goods;

    static GoodsProvider()
    {
        Goods = new List<GoodDto>();
        for (var i = 0; i < GoodsCount; i++)
        {
            Goods.Add(
                new GoodDto(
                    Id: Faker.Random.Long(99999, 9999999),
                    Name: Faker.Commerce.ProductName(),
                    Price: Math.Round(Faker.Random.Decimal(99, 2999), 2),
                    Weight: Faker.Random.UInt()));
        }
    }
    
    public GoodDto GetRandomGood()
    {
        return Goods[Faker.Random.Int(0, GoodsCount - 1)];
    }
}