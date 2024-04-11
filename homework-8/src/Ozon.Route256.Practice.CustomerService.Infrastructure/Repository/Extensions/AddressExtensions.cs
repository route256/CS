using Ozon.Route256.Practice.CustomerService.Domain.Address;
using Ozon.Route256.Practice.CustomerService.Infrastructure.Repository.Dto;

namespace Ozon.Route256.Practice.CustomerService.Infrastructure.Repository.Extensions;

internal static class AddressExtensions
{
    public static Address ToDomain(this AddressDto dto)
        => new(
            id: dto.Id,
            region: dto.Region,
            city: dto.City,
            street: dto.Street,
            building: dto.Building,
            apartment: dto.Apartment,
            customerId: dto.CustomerId,
            isDefault: dto.IsDefault,
            latitude: dto.Latitude,
            longitude: dto.Longitude);

    public static AddressDto ToDto(this Address address)
        => new()
        {
            Apartment = address.Apartment,
            Building = address.Building,
            City = address.City,
            CustomerId = address.CustomerId,
            Id = address.Id,
            IsDefault = address.IsDefault,
            Latitude = address.Latitude,
            Longitude = address.Longitude,
            Region = address.Region,
            Street = address.Street,
        };
}
