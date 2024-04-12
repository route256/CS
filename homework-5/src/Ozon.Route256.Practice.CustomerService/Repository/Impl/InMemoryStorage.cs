using System.Collections.Concurrent;
using Ozon.Route256.Practice.CustomerService.Repository.Dto;

namespace Ozon.Route256.Practice.CustomerService.Repository.Impl;

public class InMemoryStorage
{
    public readonly ConcurrentDictionary<int, CustomerDto> Customers = new(2, 10);
    public readonly ConcurrentDictionary<int, AddressDto>  Addresses = new(2, 10);

    public InMemoryStorage()
    {
        FakeCustomers();
        KafeAddresses();
    }

    private const double FirstCoordinateX = 55.7522;
    private const double FirstCoordinateY = 37.6156;

    private const double SecondCoordinateX = 55.01;
    private const double SecondCoordinateY = 82.55;

    private (double x, double y) GetCoordinates() =>
        Faker.Boolean.Random()
            ? (x: FirstCoordinateX, y: FirstCoordinateY)
            : (x: SecondCoordinateX, y: SecondCoordinateY);


    private void FakeCustomers()
    {
        var customers = Enumerable.Range(1, 100)
                                  .Select(x => new CustomerDto(
                                              x,
                                              Faker.Name.First(),
                                              Faker.Name.Last(),
                                              Faker.Phone.Number(),
                                              Faker.Internet.Email(),
                                              x,
                                              Enumerable.Range(1, 100)
                                                        .OrderBy(_ => Faker.RandomNumber.Next())
                                                        .Take(Faker.RandomNumber.Next(2, 13))
                                                        .ToArray()
                                          ));

        foreach (var c in customers)
            Customers[c.Id] = c;
    }

    private void KafeAddresses()
    {
        var regions = new[] { "Moscow", "StPetersburg", "Novosibirsk" };
        var addresses = Enumerable.Range(1, 100).Select(x =>
        {
            var coordinates = GetCoordinates();
            return new AddressDto(
                x,
                regions[Faker.RandomNumber.Next(0, 2)],
                Faker.Address.City(),
                Faker.Address.StreetName(),
                Faker.RandomNumber.Next().ToString(),
                Faker.RandomNumber.Next().ToString(),
                coordinates.x,
                coordinates.y
            );
        });

        foreach (var address in addresses)
            Addresses[address.Id] = address;
    }
}