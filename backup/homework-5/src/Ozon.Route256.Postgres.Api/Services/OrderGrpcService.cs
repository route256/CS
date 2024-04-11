using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Ozon.Route256.Postgres.Api.Mapping;
using Ozon.Route256.Postgres.Domain;
using Ozon.Route256.Postgres.Grpc;

namespace Ozon.Route256.Postgres.Api.Services;

public sealed class OrderGrpcService : OrderService.OrderServiceBase
{
    private readonly IOrderRepository _orderRepository;

    public OrderGrpcService(IOrderRepository orderRepository) => _orderRepository = orderRepository;

    public override async Task<GetResponse> Get(GetRequest request, ServerCallContext context)
    {
        var result = await _orderRepository
            .Get(request.OrderId.ToArray(), context.CancellationToken)
            .ToArrayAsync(context.CancellationToken);

        return new()
        {
            Orders = { result.Select(order => order.Map()) }
        };
    }

    public override async Task GetStream(
        GetRequest request,
        IServerStreamWriter<GetStreamResponse> responseStream,
        ServerCallContext context)
    {
        var result = _orderRepository.Get(request.OrderId.ToArray(), context.CancellationToken);

        await foreach (var order in result)
            await responseStream.WriteAsync(new() { Order = order.Map() });
    }
}
