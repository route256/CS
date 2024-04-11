using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Ozon.Route256.Postgres.Grpc;
using Ozon.Route256.Postgres.IntegrationTests.Common;
using Xunit;

namespace Ozon.Route256.Postgres.IntegrationTests;

public sealed class GetTests : IClassFixture<StartupFixture>
{
    private readonly StartupFixture _fixture;

    public GetTests(StartupFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Get_ShouldReturnOrders()
    {
        // Arrange
        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        var grpc = GetClient();
        var orderIds = await GetOrderIds(100500, 1000, cts.Token).ToArrayAsync(cts.Token);

        // Act
        var result = await grpc.GetAsync(
            new GetRequest
            {
                OrderId = { orderIds }
            },
            cancellationToken: cts.Token);

        // Assert
        Assert.Equal(orderIds.Length, result.Orders.Count);
    }

    [Fact]
    public async Task GetStream_ShouldReturnOrders()
    {
        // Arrange
        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        var grpc = GetClient();
        var orderIds = await GetOrderIds(100500, 1000, cts.Token).ToArrayAsync(cts.Token);

        // Act
        using var call = grpc.GetStream(
            new GetRequest
            {
                OrderId = { orderIds }
            },
            cancellationToken: cts.Token);

        var result = await call.ResponseStream
            .ReadAllAsync(cts.Token)
            .Select(response => response.Order)
            .ToArrayAsync(cts.Token);

        // Assert
        Assert.Equal(orderIds.Length, result.Length);
    }

    private OrderService.OrderServiceClient GetClient()
    {
        var client = _fixture.CreateClient();
        var channel = GrpcChannel.ForAddress(client.BaseAddress!, new() { HttpClient = client });
        return new(channel);
    }

    private async IAsyncEnumerable<long> GetOrderIds(
        long minOrderId,
        long limit,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var connectionString = _fixture.Services.GetRequiredService<IConfiguration>()["ConnectionString"];
        await using var connection = new NpgsqlConnection(connectionString);
        await using var command = new NpgsqlCommand("SELECT order_id FROM orders WHERE order_id > :min_order_id LIMIT :limit", connection)
        {
            Parameters =
            {
                { "min_order_id", minOrderId },
                { "limit", limit },
            },
        };
        await connection.OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
            yield return reader.GetFieldValue<long>(0);
    }
}
