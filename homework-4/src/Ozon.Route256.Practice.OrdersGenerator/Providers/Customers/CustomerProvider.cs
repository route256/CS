using Bogus;

namespace Ozon.Route256.Practice.OrdersGenerator.Providers.Customers;

public class CustomerProvider: ICustomerProvider
{
    private static readonly Faker Faker = new();
    private static readonly int CustomersCount = 50;
    private static readonly List<CustomerDto> Customers;
    
    private const double FirstCoordinateX = 55.7522;
    private const double FirstCoordinateY = 37.6156;

    private const double SecondCoordinateX = 55.01;
    private const double SecondCoordinateY = 82.55;

    private static (double x, double y) GetCoordinates() =>
        Faker.Random.Bool()
            ? (x: FirstCoordinateX, y: FirstCoordinateY)
            : (x: SecondCoordinateX, y: SecondCoordinateY);

    static CustomerProvider()
    {
        Customers = new List<CustomerDto>();
        for (var i = 1; i <= CustomersCount; i++)
        {
            Customers.Add(
                new CustomerDto(
                    i,
                    Faker.Name.FirstName(),
                    Faker.Name.LastName(),
                    FakeAddresses()));
        }
    }
    
    public Task<CustomerDto> GetRandomCustomer()
    {
        return Task.FromResult(
            Customers[Faker.Random.Int(0, CustomersCount - 1)]);
    }

    private static IEnumerable<AddressDto> FakeAddresses()
    {
        var regions = new[] { "Moscow", "StPetersburg", "Novosibirsk" };
        var addresses = Enumerable.Range(0, Faker.Random.Int(1, 3))
            .Select(x =>
            {
                var coordinates = GetCoordinates();
                return new AddressDto(
                    regions[Faker.Random.Number(0, 2)],
                    Faker.Address.City(),
                    Faker.Address.StreetName(),
                    Faker.Random.Number().ToString(),
                    Faker.Random.Number().ToString(),
                    coordinates.x,
                    coordinates.y
                );
            })
            .ToArray();

        return addresses;
    }
}
