using Ozon.Route256.Practice.CustomerService.Domain.Exceptions;

namespace Ozon.Route256.Practice.CustomerService.Domain.Customer;

/// <summary>
/// Represents Customer class.
/// </summary>
public sealed class CustomerAggregate
{
    public CustomerAggregate(
        Customer customer,
        IEnumerable<Address.Address> addresses)
    {
        Customer = customer;
        SetAddresses(addresses);
    }

    public CustomerAggregate(
        Customer customer,
        Address.Address defaultAddress,
        IEnumerable<Address.Address> addresses)
    {
        Customer = customer;
        DefaultAddress = defaultAddress;
        Addresses = addresses.ToList();
    }

    public Customer Customer { get; }

    public Address.Address DefaultAddress { get; private set; }

    public IReadOnlyCollection<Address.Address> Addresses { get; private set; } = new List<Address.Address>();

    private void SetAddresses(IEnumerable<Address.Address> addresses)
    {
        var customerAddresses = addresses.Where(address => address.CustomerId == Customer.Id).ToList();

        if (customerAddresses.Count == 0)
        {
            throw new AddressNotFoundException("Addresses were not found for customer");
        }

        var defaultAddress = customerAddresses.Where(address => address.IsDefault).ToList();

        if (defaultAddress.Count == 0)
        {
            throw new AddressNotFoundException("Default address was not found for customer");
        }

        if (defaultAddress.Count > 1)
        {
            throw new AddressNotFoundException("More than one default address was found for customer");
        }

        DefaultAddress = defaultAddress.First();

        Addresses = customerAddresses.Where(address => !address.IsDefault).ToList();
    }
}
