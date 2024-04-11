using Bogus;
using Microsoft.Extensions.Options;
using Ozon.Route256.Practice.OrdersGenerator.Configuration;
using Ozon.Route256.Practice.OrdersGenerator.Infrastructure.Kafka;
using Ozon.Route256.Practice.OrdersGenerator.Models;
using Ozon.Route256.Practice.OrdersGenerator.Providers.Customers;
using Ozon.Route256.Practice.OrdersGenerator.Providers.Goods;

namespace Ozon.Route256.Practice.OrdersGenerator.Generator;

public class OrderGenerator: IOrderGenerator
{
    private readonly Faker _faker = new();
    
    private readonly ICustomerProvider _customerProvider;
    private readonly IGoodsProvider _goodsProvider;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly OrderGeneratorSettings _settings;

    public OrderGenerator(
        ICustomerProvider customerProvider,
        IKafkaProducer kafkaProducer,
        IGoodsProvider goodsProvider,
        IOptions<OrderGeneratorSettings> options)
    {
        _customerProvider = customerProvider;
        _kafkaProducer = kafkaProducer;
        _goodsProvider = goodsProvider;
        _settings = options.Value;
    }

    public async Task GenerateOrder(
        CancellationToken token)
    {
        var orderId = _faker.Random.Long(999999, 99999999);
        var source = _settings.OrderSource;
        var customer = await _customerProvider.GetRandomCustomer();

        var address = _faker.Random
            .ArrayElements(customer.Addresses.ToArray(), 1)
            .Select(x => new AddressModel(
                Region: x.Region,
                City: x.City,
                Street: x.Street,
                Building: x.Building,
                Apartment: x.Apartment,
                Latitude: x.Latitude,
                Longitude: x.Longitude))
            .First();
        
        var customerDto = new CustomerModel(
            customer.Id,
            address);
        
        var goods = Enumerable.Range(0, _faker.Random.Int(1, 3))
            .Select(_ =>
            {
                var good = _goodsProvider.GetRandomGood();
                return new GoodModel(
                    Id: good.Id,
                    Name: good.Name,
                    Quantity: _faker.Random.Int(1, 10),
                    Price: good.Price,
                    Weight: good.Weight);
            });

        var order = new OrderModel(
            orderId,
            source,
            customerDto,
            goods);
        
        await _kafkaProducer.SendMessage(
            _settings.OrderRequestTopic,
            orderId,
            order,
            token);
    }
}