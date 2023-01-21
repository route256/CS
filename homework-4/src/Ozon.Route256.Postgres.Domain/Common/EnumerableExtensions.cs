using System;
using System.Collections.Generic;
using System.Linq;

namespace Ozon.Route256.Postgres.Domain.Common;

public static class EnumerableExtensions
{
    public static TResult[] ToArrayBy<TSource, TResult>(this IEnumerable<TSource> collection, Func<TSource, TResult> selector) =>
        collection.Select(selector).ToArray();
}
