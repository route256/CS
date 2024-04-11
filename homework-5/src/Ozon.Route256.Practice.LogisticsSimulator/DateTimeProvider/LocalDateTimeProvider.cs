namespace Ozon.Route256.Practice.LogisticsSimulator.DateTimeProvider;

public class LocalDateTimeProvider: IDateTimeProvider
{
    public DateTimeOffset CurrentDateTimeOffsetUtc => DateTimeOffset.UtcNow;
}