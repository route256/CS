using Ozon.Route256.Practice.CustomerService.Observation;
using Prometheus;

namespace Ozon.Route256.Practice.CustomerService.Infrastructure.Metrics;

internal sealed class GrpcMetrics : IGrpcMetrics
{
    private readonly Histogram _histogram = Prometheus.Metrics.CreateHistogram(
        name: "customerservice_grpc_response_time",
        help: "Время ответа сервиса",
        labelNames: new[] { "methodName", "isError" });

    /// <inheritdoc />
    public void WriteResponseTime(long elapsedMilliseconds, string methodName, bool isError) =>
        _histogram.WithLabels(methodName, isError ? "1" : "0").Observe((double)elapsedMilliseconds / 1000);
}