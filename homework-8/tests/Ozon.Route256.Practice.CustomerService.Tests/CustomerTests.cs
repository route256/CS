using Microsoft.Extensions.Logging;
using Moq;
using Ozon.Route256.Practice.CustomerService.Application;
using Ozon.Route256.Practice.CustomerService.Domain.Address;
using Ozon.Route256.Practice.CustomerService.Domain.Customer;
using Ozon.Route256.Practice.CustomerService.Domain.Exceptions;
using Ozon.Route256.Practice.CustomerService.Observation;

namespace Ozon.Route256.Practice.CustomerService.Tests;

public class CustomerTests
{
    [Fact]
    public void NoDefaultAddress()
    {
        var customer = new Customer(
            id: 1,
            firstName: "",
            lastName: "",
            mobileNumber: "",
            email: "");

        Assert.Throws<AddressNotFoundException>(
            () =>
                new CustomerAggregate(customer, new Address[]
                {
                    new(1, "r", "c", "s", "b", "a", 1, false, 1d, 1d)
                }));
    }

    [Fact]
    public void HaveDefaultAddress()
    {
        var customer = new Customer(
            id: 1,
            firstName: "",
            lastName: "",
            mobileNumber: "",
            email: "");

        var ca = new CustomerAggregate(customer, new Address[]
        {
            new(1, "r", "c", "s", "b", "a", 1, true, 1d, 1d)
        });

        Assert.NotNull(ca.DefaultAddress);
        Assert.Empty(ca.Addresses);
    }
    
    [Fact]
    public void HaveDefaultAndNotDefaultAddress()
    {
        var customer = new Customer(
            id: 1,
            firstName: "",
            lastName: "",
            mobileNumber: "",
            email: "");

        var ca = new CustomerAggregate(customer, new Address[]
        {
            new(1, "r", "c", "s", "b", "a", 1, true, 1d, 1d),
            new(2, "r", "c", "s", "b", "a", 1, false, 1d, 1d)
        });

        Assert.NotNull(ca.DefaultAddress);
        Assert.NotEmpty(ca.Addresses);
    }
    
    [Fact]
    public void DifferentAddressesSameIDAddress()
    {
        var customer = new Customer(
            id: 1,
            firstName: "",
            lastName: "",
            mobileNumber: "",
            email: "");

        Assert.Throws<DuplicatedAddressException>(() =>
        {
            new CustomerAggregate(customer, new Address[]
            {
                new(1, "r", "c", "s", "b", "a", 1, true, 1d, 1d),
                new(1, "r", "c", "s2", "b2", "a", 1, false, 1d, 1d)
            });
        });
    }

    [Theory]
    [MemberData(nameof(Parameters))]
    public async Task GetCustomerTest(int expectedCustomerId, Customer expectedCustomer, Address[] addresses)
    {
        var customerRepo = new Mock<ICustomerRepository>();

        customerRepo
           .Setup(x => x.Find(expectedCustomerId, It.IsAny<CancellationToken>()))
           .ReturnsAsync(expectedCustomer);

        var addressRepo = new Mock<IAddressRepository>();

        addressRepo
           .Setup(x => x.FindAllForCustomer(expectedCustomerId, It.IsAny<CancellationToken>()))
           .ReturnsAsync(addresses);

        var uow     = new Mock<IUnitOfWork>();
        var logger  = new Mock<ILogger<Application.CustomerService>>();
        var metrics = new Mock<IBusinessMetrics>();

        var customerService = new Application.CustomerService(customerRepo.Object, addressRepo.Object, uow.Object, logger.Object, metrics.Object);

        var customer = await customerService.GetCustomerById(expectedCustomerId, CancellationToken.None);

        Assert.NotNull(customer);
        Assert.Equal(expectedCustomerId, customer.Customer.Id);
    }

    public static IEnumerable<object[]> Parameters()
    {
        yield return new object[]
        {
            1,
            new Customer(
                id: 1,
                firstName: "FirstName",
                lastName: "LastName",
                mobileNumber: "Mobile",
                email: "Email"),
            new Address[]
            {
                new(1, "r1", "c1", "s1", "b1", "a1", 1, false, 1d, 1d),
                new(2, "r2", "c2", "s2", "b2", "a2", 1, true, 2d, 2d),
            }
        };

        yield return new object[]
        {
            2,
            new Customer(
                id: 2,
                firstName: "FirstName",
                lastName: "LastName",
                mobileNumber: "Mobile",
                email: "Email"),
            new Address[]
            {
                new(1, "r1", "c1", "s1", "b1", "a1", 1, false, 1d, 1d),
                new(2, "r2", "c2", "s2", "b2", "a2", 1, true, 2d, 2d),
            }
        };
    }
}