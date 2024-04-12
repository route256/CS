using System.Diagnostics;

namespace Ozon.Route256.Practice.CustomerService.Infrastructure.Tracing;

public sealed class CustomerActivitySource : ICustomerActivitySource
{
    public const string ActivityName = "CustomerActivitySource";

    public ActivitySource ActivitySource => new(ActivityName);
}