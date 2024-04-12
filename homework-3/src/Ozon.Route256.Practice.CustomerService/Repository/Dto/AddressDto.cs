namespace Ozon.Route256.Practice.CustomerService.Repository.Dto;

public record AddressDto(
    int    Id,
    string Region,
    string City,
    string Street,
    string Building,
    string Apartment,
    double Latitude,
    double Longitude
);