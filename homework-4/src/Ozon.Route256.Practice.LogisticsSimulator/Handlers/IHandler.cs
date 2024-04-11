using Ozon.Route256.Practice.LogisticsSimulator.Handlers.ResultTypes;

namespace Ozon.Route256.Practice.LogisticsSimulator.Handlers;


public interface IHandler<in TRequest, TResponse>
{
    Task<HandlerResult<TResponse>> Handle(TRequest request, CancellationToken token);
}

public interface IHandler<in TRequest>
{
    Task<HandlerResult> Handle(TRequest request, CancellationToken token);
}

public interface IHandler : IHandler<Unit>
{
    Task<HandlerResult> IHandler<Unit>.Handle(Unit request, CancellationToken token) => Handle(token);
    Task<HandlerResult> Handle(CancellationToken token);
}

public record struct Unit;