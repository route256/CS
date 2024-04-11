namespace Ozon.Route256.Practice.LogisticsSimulator.DateTimeProvider;

public interface IDateTimeProvider
{
    DateTimeOffset CurrentDateTimeOffsetUtc { get; }
}