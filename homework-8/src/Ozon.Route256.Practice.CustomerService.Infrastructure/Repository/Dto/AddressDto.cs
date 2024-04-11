namespace Ozon.Route256.Practice.CustomerService.Infrastructure.Repository.Dto;

internal sealed class AddressDto
{
    public int Id { get; init; }
    public int  CustomerId { get; init; }
    public bool IsDefault { get; init; }
    public string Region { get; init; }
    public string City { get; init; }
    public string Street { get; init; }
    public string Building { get; init; }
    public string Apartment { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
}
