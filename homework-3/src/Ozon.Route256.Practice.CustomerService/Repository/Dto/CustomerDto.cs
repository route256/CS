namespace Ozon.Route256.Practice.CustomerService.Repository.Dto;

public record CustomerDto(
    int    Id,
    string FirstName,
    string LastName,
    string MobileNumber,
    string Email,
    int    AddressID,
    int[]  Addresses
);