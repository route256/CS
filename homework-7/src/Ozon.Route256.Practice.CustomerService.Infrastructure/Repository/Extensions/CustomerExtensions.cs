using Ozon.Route256.Practice.CustomerService.Domain.Customer;
using Ozon.Route256.Practice.CustomerService.Infrastructure.Repository.Dto;

namespace Ozon.Route256.Practice.CustomerService.Infrastructure.Repository.Extensions;

/// <summary>
/// Represents CustomerExtensions class.
/// </summary>
internal static class CustomerExtensions
{
    public static Customer ToDomain(this CustomerDto dto)
        => new (
            id: dto.Id,
            firstName: dto.FirstName,
            lastName: dto.LastName,
            mobileNumber: dto.MobileNumber,
            email: dto.Email);

    public static CustomerDto ToDto(this Customer customer)
        => new()
        {
            Email = customer.Email,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Id = customer.Id,
            MobileNumber = customer.MobileNumber,
        };
}
