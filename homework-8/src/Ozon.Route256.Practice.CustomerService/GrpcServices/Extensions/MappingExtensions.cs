using Ozon.Route256.Practice.CustomerService.Domain.Customer;

namespace Ozon.Route256.Practice.CustomerService.GrpcServices.Extensions;

public static class MappingExtensions
{
    public static Customer ToProtoCustomer(this CustomerAggregate customer)
    {
        var defaultAddress = customer.DefaultAddress.ToProtoAddress();

        var extraAddresses = customer.Addresses.Select(ToProtoAddress).ToArray();

        return new()
        {
            Id = customer.Customer.Id,
            FirstName = customer.Customer.FirstName,
            LastName = customer.Customer.LastName,
            MobileNumber = customer.Customer.MobileNumber,
            Email = customer.Customer.Email,
            DefaultAddress = defaultAddress,
            Addresses = { extraAddresses }
        };
    }

    public static CustomerAggregate ToDomainCustomer(this Customer customer)
    {
        var domainCustomer = new Domain.Customer.Customer(
            id: customer.Id,
            firstName: customer.FirstName,
            lastName: customer.LastName,
            mobileNumber: customer.MobileNumber,
            email: customer.Email);

        var defaultAddress = customer.DefaultAddress.ToDomainAddress(customer.Id, true);
        var addresses = customer.Addresses.Select(address => address.ToDomainAddress(customer.Id, false)).ToList();

        return new CustomerAggregate(domainCustomer, defaultAddress, addresses);
    }

    private static Domain.Address.Address ToDomainAddress(this Address address, int customerId, bool isDefault)
        => new (
            id: 0,
            region: address.Region,
            city: address.City,
            street: address.Street,
            building: address.Building,
            apartment: address.Apartment,
            customerId: customerId,
            isDefault: isDefault,
            latitude: address.Latitude,
            longitude: address.Longitude);

    public static Address ToProtoAddress(this Domain.Address.Address address)
        => new()
        {
            Region = address.Region,
            City = address.City,
            Street = address.Street,
            Building = address.Building,
            Apartment = address.Apartment,
            Longitude = address.Longitude,
            Latitude = address.Latitude
        };
}
