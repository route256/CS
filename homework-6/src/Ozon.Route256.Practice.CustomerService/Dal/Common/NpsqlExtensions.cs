using System.Data.Common;
using Npgsql;

namespace Ozon.Route256.Practice.CustomerService.Dal.Common;

public static class NpsqlExtensions
{
    public static void Add<T>(this DbParameterCollection parameters, string name, T? value) =>
        parameters.Add(
            new NpgsqlParameter<T>
            {
                ParameterName = name,
                TypedValue    = value
            });
}