using Bogus;

namespace Ozon.Route256.Practice.OrdersGenerator.Providers.Customers;

public class CustomerProvider: ICustomerProvider
{
    private static readonly Faker Faker = new();
    private static readonly int CustomersCount = 50;
    private static readonly List<CustomerDto> Customers;

    static CustomerProvider()
    {
        Customers = new List<CustomerDto>();
        for (var i = 0; i < CustomersCount; i++)
        {
            Customers.Add(
                new CustomerDto(
                    Faker.Random.Long(99999, 9999999),
                    Faker.Name.FirstName(),
                    Faker.Name.LastName(),
                    Enumerable.Range(0, Faker.Random.Int(1, 3))
                        .Select(_ => 
                            new AddressDto(
                                Region: Faker.Address.State(),
                                City: Faker.Address.City(),
                                Street: Faker.Address.StreetName(),
                                Building: Faker.Address.BuildingNumber(),
                                Apartment: Faker.Random.Int(9, 999).ToString(),
                                Latitude: Faker.Address.Latitude(),
                                Longitude: Faker.Address.Longitude()))));
        }
    }
    
    public Task<CustomerDto> GetRandomCustomer()
    {
        return Task.FromResult(
            Customers[Faker.Random.Int(0, CustomersCount - 1)]);
    }
}