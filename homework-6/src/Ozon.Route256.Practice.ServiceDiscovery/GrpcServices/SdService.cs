using Grpc.Core;

namespace Ozon.Route256.Practice.ServiceDiscovery.GrpcServices;

public class SdServiceController : SdService.SdServiceBase
{
    private readonly IResourceStore _resourceStore;
    private readonly ILogger<SdServiceController> _logger;

    public SdServiceController(IResourceStore resourceStore, ILogger<SdServiceController> logger)
    {
        _resourceStore = resourceStore;
        _logger = logger;
    }

    public override async Task DbResources(
        DbResourcesRequest request,
        IServerStreamWriter<DbResourcesResponse> responseStream,
        ServerCallContext context)
    {
        if (!_resourceStore.Contains(request.ClusterName))
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Имя кластера не найдено"));
        }

        var tcs = new CompletionSource(responseStream, context.CancellationToken);

        _resourceStore.Append(request.ClusterName, tcs);

        try
        {
            await tcs.Task;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Обновление данных по кластеру {DbCluster} было прекращено со стороны клиента", request.ClusterName);
        }
        catch (RpcException e)
        {
            _logger.LogWarning(e, "Ошибка обработки потока");

            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);

            throw;
        }
        finally
        {
            _resourceStore.Remove(request.ClusterName, tcs); //удаляем из списков на обновление
        }
    }
}