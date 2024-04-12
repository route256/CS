namespace Ozon.Route256.Practice.OrdersGenerator.Providers.Customers;

public record AddressDto(
    string Region,
    string City,
    string Street,
    string Building,
    string Apartment,
    double Latitude,
    double Longitude);