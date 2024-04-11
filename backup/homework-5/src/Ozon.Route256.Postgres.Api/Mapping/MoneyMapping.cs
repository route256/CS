using Google.Type;

namespace Ozon.Route256.Postgres.Api.Mapping;

internal static class MoneyMapping
{
    private const decimal NanoFactor = 1_000_000_000;

    public static decimal ToDecimal(this Money money) => money.Units + money.Nanos / NanoFactor;

    public static Money ToMoney(this decimal value)
    {
        var units = decimal.ToInt64(value);
        var nanos = decimal.ToInt32((value - units) * NanoFactor);

        return new() { Units = units, Nanos = nanos };
    }
}
