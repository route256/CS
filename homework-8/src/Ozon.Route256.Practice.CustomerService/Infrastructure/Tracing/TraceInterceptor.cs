using System.Diagnostics;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Ozon.Route256.Practice.CustomerService.Infrastructure.Tracing;

public class TraceInterceptor : Interceptor
{
    private readonly ICustomerActivitySource _customerActivitySource;

    /// <inheritdoc />
    public TraceInterceptor(ICustomerActivitySource customerActivitySource)
        => _customerActivitySource = customerActivitySource;

    /// <inheritdoc />
    public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest                               request, ServerCallContext context,
                                                                            UnaryServerMethod<TRequest, TResponse> continuation)
    {
        using var activity = _customerActivitySource.ActivitySource.StartActivity(
            name: context.Method,
            kind: ActivityKind.Internal,
            tags: new List<KeyValuePair<string, object?>>()
            {
                new ("request", request),
            }
        );


        return base.UnaryServerHandler(request, context, continuation);
    }
}