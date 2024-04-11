namespace Ozon.Route256.Practice.CustomerService.Observation;

public interface IGrpcMetrics
{
    void WriteResponseTime(long elapsedMilliseconds, string methodName, bool isError);
}