using Grpc.Core;

namespace Ozon.Route256.Practice.ServiceDiscovery;

public sealed class CompletionSource : TaskCompletionSource<IServerStreamWriter<DbResourcesResponse>>
{
    public IServerStreamWriter<DbResourcesResponse> ResponseStream { get; }
    public CompletionSource(IServerStreamWriter<DbResourcesResponse> responseStream, CancellationToken token)
    {
        ResponseStream = responseStream;
        token.Register(SetCanceled);
    }
}