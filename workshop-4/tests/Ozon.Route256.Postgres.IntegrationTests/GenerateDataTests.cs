using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Postgres.Domain;
using Ozon.Route256.Postgres.Domain.Common;
using Ozon.Route256.Postgres.IntegrationTests.Common;
using Xunit;

namespace Ozon.Route256.Postgres.IntegrationTests;

public sealed class GenerateDataTests : IClassFixture<StartupFixture>
{
    private readonly IOrderRepository _repository;

    public GenerateDataTests(StartupFixture fixture)
    {
        var scope = fixture.Services.CreateScope();
        _repository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
    }

    [Fact]
    public async Task Generate()
    {
        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));
        using var clients = ClientIds.AsEnumerable().GetEnumerator();
        using var skus = SkuIds.AsEnumerable().GetEnumerator();
        foreach (var (orderState, (min, max)) in Data)
        {
            var orders = Enumerable
                .Range(1, s_random.Next(min, max))
                .Select(
                    _ =>
                    {
                        if (!clients.MoveNext())
                        {
                            clients.Reset();
                            clients.MoveNext();
                        }

                        var items = Enumerable
                            .Range(1, s_random.Next(1, 15))
                            .Select(
                                _ =>
                                {
                                    if (!skus.MoveNext())
                                    {
                                        skus.Reset();
                                        skus.MoveNext();
                                    }

                                    return new Order.Item(skus.Current.SkuId, s_random.Next(1, 10), skus.Current.Price);
                                })
                            .ToArray();
                        return new Order(
                            default,
                            clients.Current,
                            orderState,
                            items.Sum(item => item.Price * item.Quantity),
                            DateTimeOffset.UtcNow,
                            items);
                    })
                .ToArray();

            foreach (var orderChunk in orders.Chunk(1000))
                await _repository.Add(orderChunk, cts.Token);
        }
    }

    private static readonly Random s_random = new();

    private static readonly long[] ClientIds = Enumerable
        .Range(10000, 100000)
        .OrderBy(_ => s_random.Next(100000))
        .ToArrayBy(i => (long)i);

    private static readonly (long SkuId, decimal Price)[] SkuIds = Enumerable
        .Range(100_000, 2_000_000)
        .OrderBy(_ => s_random.Next(2_000_000))
        .ToArrayBy(i => ((long)i, (decimal)s_random.Next(100, 5000)));

    private static IReadOnlyDictionary<OrderState, (int Min, int Max)> Data = new Dictionary<OrderState, (int Min, int Max)>()
    {
        { OrderState.Created, (30_000, 100_000) },
        { OrderState.Paid, (30_000, 100_000) },
        { OrderState.Boxing, (30_000, 100_000) },
        { OrderState.WaitForPickup, (30_000, 100_000) },
        { OrderState.InDelivery, (30_000, 100_000) },
        { OrderState.WaitForClient, (30_000, 100_000) },
        { OrderState.Completed, (30_000, 100_000) },
        { OrderState.Cancelled, (30_000, 100_000) },
    };
}
