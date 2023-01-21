using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Ozon.WebApp.Services;

internal sealed class ServerLoggerInterceptor: Interceptor
{
    private readonly ILogger<ServerLoggerInterceptor> _logger;

    public ServerLoggerInterceptor(ILogger<ServerLoggerInterceptor> logger)
    {
        _logger = logger;
    }

    public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        _logger.LogInformation("Call {method}", context.Method);
        var x = continuation(request, context);
        _logger.LogInformation("Call {method}", context.Method);
        
        return base.UnaryServerHandler(request, context, continuation);
    }
}