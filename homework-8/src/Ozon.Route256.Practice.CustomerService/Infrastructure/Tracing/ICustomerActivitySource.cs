using System.Diagnostics;

namespace Ozon.Route256.Practice.CustomerService.Infrastructure.Tracing;

public interface ICustomerActivitySource
{
    ActivitySource ActivitySource { get; }
}