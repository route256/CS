using Grpc.Core;
using Ozon.Route256.Practice.LogisticsSimulator.Grpc;
using Ozon.Route256.Practice.LogisticsSimulator.Handlers.ResultTypes;
using Ozon.Route256.Practice.LogisticsSimulator.Handlers.OrderCancel;

namespace Ozon.Route256.Practice.LogisticsSimulator.Infrastructure.Grpc;

public class LogisticsSimulatorGrpcService: LogisticsSimulatorService.LogisticsSimulatorServiceBase
{
    private readonly IOrderCancellationHandler _handler;
    
    public LogisticsSimulatorGrpcService(IOrderCancellationHandler handler)
    {
        _handler = handler;
    }

    public override async Task<CancelResult> OrderCancel(Order request, ServerCallContext context)
    {
        var result = await _handler.Handle(new IOrderCancellationHandler.Request(request.Id), context.CancellationToken);
        return result.Handle(
            () => new CancelResult { Success = true},
            e => new CancelResult{ Success = false, Error = e.BusinessError}
        );
    }
}