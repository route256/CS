namespace Ozon.Route256.Practice.CustomerService.Domain.Exceptions;

/// <summary>
/// Represents AddressNotFoundException class.
/// </summary>
public sealed class AddressNotFoundException : Exception
{
    public AddressNotFoundException(string message)
        : base(message)
    {
    }
}