using Ozon.Route256.Practice.CustomerService.Repository.Dto;

namespace Ozon.Route256.Practice.CustomerService.GrpcServices.Extensions;

public static class MappingExtensions
{
    public static Address ToProtoAddress(this AddressDto addressDto) =>
        new()
        {
            Region    = addressDto.Region,
            City      = addressDto.City,
            Street    = addressDto.Street,
            Building  = addressDto.Building,
            Apartment = addressDto.Apartment,
            Longitude = addressDto.Longitude,
            Latitude  = addressDto.Latitude
        };

    public static Customer ToProtoCustomer(
        this CustomerDto customerDto,
        AddressDto[] addresses)
    {
        var defaultAddress = addresses.FirstOrDefault(x => x.IsDefault).ToProtoAddress();
        var extraAddreses = addresses.Where(x => !x.IsDefault).Select(ToProtoAddress).ToArray();
        return new Customer()
        {
            Id             = customerDto.Id,
            FirstName      = customerDto.FirstName,
            LastName       = customerDto.LastName,
            MobileNumber   = customerDto.MobileNumber,
            Email          = customerDto.Email,
            DefaultAddress = defaultAddress,
            Addresses      = { extraAddreses }
        };
    }
}