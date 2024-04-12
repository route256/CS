using System.Diagnostics;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Ozon.Route256.Practice.CustomerService.Observation;

namespace Ozon.Route256.Practice.CustomerService.Infrastructure.Metrics;

public sealed class MetricsInterceptor : Interceptor
{
    private readonly IGrpcMetrics _grpcMetrics;

    /// <inheritdoc />
    public MetricsInterceptor(IGrpcMetrics grpcMetrics)
        => _grpcMetrics = grpcMetrics;

    /// <inheritdoc />
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = await base.UnaryServerHandler(request, context, continuation);

            stopwatch.Stop();

            _grpcMetrics.WriteResponseTime(stopwatch.ElapsedMilliseconds, context.Method, isError: false);

            return result;
        }
        catch
        {
            stopwatch.Stop();

            _grpcMetrics.WriteResponseTime(stopwatch.ElapsedMilliseconds, context.Method, isError: true);

            throw;
        }
    }
}