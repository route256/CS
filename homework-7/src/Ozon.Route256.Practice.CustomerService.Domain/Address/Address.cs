namespace Ozon.Route256.Practice.CustomerService.Domain.Address;

/// <summary>
/// Represents Address class.
/// </summary>
public sealed class Address
{
    public Address(int id, string region, string city, string street, string building, string apartment, int customerId, bool isDefault, double latitude, double longitude)
    {
        Id = id;
        Region = region;
        City = city;
        Street = street;
        Building = building;
        Apartment = apartment;
        CustomerId = customerId;
        IsDefault = isDefault;
        Latitude = latitude;
        Longitude = longitude;
    }

    public int Id { get; }

    public int CustomerId { get; }

    public bool IsDefault { get; }

    public string Region { get; }

    public string City { get; }

    public string Street { get; }

    public string Building { get; }

    public string Apartment { get; }

    public double Latitude { get; }

    public double Longitude { get; }
}
