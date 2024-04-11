namespace Ozon.Route256.Practice.CustomerService.Observation;

public interface ICustomerInstrumentation
{
    IDisposable StartGetCustomerActivity(int customerId);

    void GetCustomerActivityFailed(int customerId);
}