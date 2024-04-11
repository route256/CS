using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ozon.WebApp.Entities;

namespace Ozon.WebApp.Services;

internal sealed class StreamingService
{
    private readonly ProductStore _store;

    public StreamingService(ProductStore store)
    {
        _store = store;
    }

    public async Task ProcessStream(IAsyncStreamReader<AddProductRequest> requestStream, IServerStreamWriter<Empty> responseStream,
        ServerCallContext context)
    {
        var x = RequestTask(requestStream);

        var y = ResponseTask(responseStream, context);

        await Task.WhenAny(x, y);
    }

    private static async Task ResponseTask(IServerStreamWriter<Empty> responseStream, ServerCallContext context)
    {
        while (!context.CancellationToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            await responseStream.WriteAsync(new Empty());
        }
    }

    private async Task RequestTask(IAsyncStreamReader<AddProductRequest> requestStream)
    {
        await foreach (var request in requestStream.ReadAllAsync())
        {
            _store.Add(new Product(request.Name)
            {
                Id = new Guid(request.Id.ToByteArray()),
                // Tags = request.Tags.ToArray(),
                // IsSponsored = request.IsSponsored,
                // Adult = request.Adult,
            });
        }
    }
}