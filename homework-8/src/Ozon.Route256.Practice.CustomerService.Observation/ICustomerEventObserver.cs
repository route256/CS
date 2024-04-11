namespace Ozon.Route256.Practice.CustomerService.Observation;

public interface ICustomerEventObserver
{
    void PublishEvent<T>(ref T evt) where T : ICustomerEvent;
}

public interface ICustomerEvent
{
}

public struct GetCustomerByIdStart : ICustomerEvent
{
    private readonly int _customerId;

    public GetCustomerByIdStart(int customerId)
    {
        _customerId = customerId;
    }
}

public struct GetCustomerByIdFailed : ICustomerEvent
{
    private readonly int _customerId;

    public GetCustomerByIdFailed(int customerId)
    {
        _customerId = customerId;
    }
}

public struct GetCustomerByIdEnded : ICustomerEvent
{
    private readonly int _customerId;

    public GetCustomerByIdEnded(int customerId)
    {
        _customerId = customerId;
    }
}