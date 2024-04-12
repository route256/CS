using Ozon.Route256.Practice.CustomerService.Observation;
using Prometheus;

namespace Ozon.Route256.Practice.CustomerService.Infrastructure.Metrics;

internal class BusinessMetrics : IBusinessMetrics
{
    private Counter _counter = Prometheus.Metrics.CreateCounter(
        name: "customerservice_customer_created",
        help: "",
        labelNames: new[] { "region" });

    /// <inheritdoc />
    public void CustomerCreated(string region)
        => _counter.WithLabels(region).Inc();
}