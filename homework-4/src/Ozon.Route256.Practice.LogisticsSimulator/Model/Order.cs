namespace Ozon.Route256.Practice.LogisticsSimulator.Model;

public record struct Order(long OrderId, OrderState OrderState, DateTimeOffset ChangedAt);