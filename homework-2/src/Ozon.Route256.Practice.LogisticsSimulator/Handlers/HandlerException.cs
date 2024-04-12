namespace Ozon.Route256.Practice.LogisticsSimulator.Handlers;

public class HandlerException: Exception
{
    protected HandlerException(string businessError): base($"Handler failed with business error: \"{businessError}\"")
    {
        BusinessError = businessError;
    }

    public string BusinessError { get; }
}