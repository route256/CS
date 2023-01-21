using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Ozon.WebApp.Services;

internal sealed class ProductGrpcService: ProductService.ProductServiceBase
{
    private readonly StreamingService _streamingService;

    public ProductGrpcService(StreamingService streamingService)
    {
        _streamingService = streamingService;
    }

    public override async Task AddProduct(
        IAsyncStreamReader<AddProductRequest> requestStream,
        IServerStreamWriter<Empty> responseStream, 
        ServerCallContext context)
    {
        await _streamingService.ProcessStream(requestStream, responseStream, context);
    }
}