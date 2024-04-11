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

    public static Customer ToProtoCustomer(this CustomerDto     customerDto,
                                           Address              defaultAddress,
                                           IEnumerable<Address> addresses) =>
        new()
        {
            Id             = customerDto.Id,
            FirstName      = customerDto.FirstName,
            LastName       = customerDto.LastName,
            MobileNumber   = customerDto.MobileNumber,
            Email          = customerDto.Email,
            DefaultAddress = defaultAddress,
            Addresses      = { addresses }
        };
}