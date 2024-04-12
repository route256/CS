namespace Ozon.Route256.Practice.CustomerService.Domain.Customer;

/// <summary>
/// Represents customer entity
/// </summary>
public sealed class Customer
{
    public Customer(
        int id,
        string firstName,
        string lastName,
        string mobileNumber,
        string email)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        MobileNumber = mobileNumber;
        Email = email;
    }

    public int Id { get; }

    public string FirstName { get; }

    public string LastName { get; }

    public string MobileNumber { get; }

    public string Email { get; }
}
