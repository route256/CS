using Grpc.Core;
using Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Repositories;

namespace Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Grpc;

public static class ExceptionHelper
{
    public static RpcException Handle<T>(this Exception exception, ServerCallContext context, ILogger<T> logger, Guid correlationId) =>
        exception switch
        {
            TimeoutException timeoutException => HandleTimeoutException(timeoutException, logger, correlationId),
            RepositoryException repositoryException => HandleRepositoryException(repositoryException, logger, correlationId),
            RpcException rpcException => HandleRpcException(rpcException, logger, correlationId),
            _ => HandleDefault(exception, logger, correlationId)
        };

    private static RpcException HandleTimeoutException(Exception exception, ILogger logger, Guid correlationId)
    {
        logger.LogError(exception, $"CorrelationId: {correlationId} - A timeout occurred");

        var status = new Status(StatusCode.Internal, "An external resource did not answer within the time limit");

        return new RpcException(status, CreateTrailers(correlationId));
    }

    private static RpcException HandleRepositoryException(Exception exception, ILogger logger, Guid correlationId)
    {
        logger.LogError(exception, "CorrelationId: {CorrelationId} - An SQL error occurred", correlationId);
        Status status;

        status = new Status(StatusCode.Internal, "Repository error");
        return new RpcException(status, CreateTrailers(correlationId));
    }

    private static RpcException HandleRpcException(RpcException exception, ILogger logger, Guid correlationId)
    {
        logger.LogError(exception, "CorrelationId: {CorrelationId} - An error occurred", correlationId);
        var trailers = exception.Trailers;
        trailers.Add(CreateTrailers(correlationId)[0]);
        return new RpcException(new Status(exception.StatusCode, exception.Message), trailers);
    }

    private static RpcException HandleDefault(Exception exception, ILogger logger, Guid correlationId)
    {
        logger.LogError(exception, "CorrelationId: {CorrelationId} - An error occurred", correlationId);
        return new RpcException(new Status(StatusCode.Internal, exception.Message), CreateTrailers(correlationId));
    }
    
    private static Metadata CreateTrailers(Guid correlationId)
    {
        var trailers = new Metadata { { "CorrelationId", correlationId.ToString() } };
        return trailers;
    }
}