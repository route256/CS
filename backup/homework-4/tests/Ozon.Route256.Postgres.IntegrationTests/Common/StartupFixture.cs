using System;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Processors;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Npgsql;
using Ozon.Route256.Postgres.Api;

namespace Ozon.Route256.Postgres.IntegrationTests.Common;

public sealed class StartupFixture : WebApplicationFactory<Startup>
{
    /// <summary>
    /// Object used to prevent migration's run in parallel
    /// because it's not thread safe
    /// </summary>
    private static readonly object s_migrationLocker = new();

    protected override IHostBuilder? CreateHostBuilder()
    {
        var hostBuilder = base.CreateHostBuilder();

        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
            hostBuilder.UseEnvironment("Development");

        return hostBuilder;
    }

    public StartupFixture()
    {
        // Accessing property `Server` starts test server
        if (Server is null)
            throw new InvalidOperationException("Unable to start TestServer");

        TryMigrate();
    }

    private void TryMigrate()
    {
        using var scope = Services.CreateScope();
        if (scope.ServiceProvider.GetService<IMigrationRunner>() is not { } migrationRunner)
            return;

        lock (s_migrationLocker)
            migrationRunner.MigrateUp();

        var dbOptions = scope.ServiceProvider.GetRequiredService<IOptions<ProcessorOptions>>();
        using var connection = new NpgsqlConnection(dbOptions.Value.ConnectionString);
        connection.Open();
        connection.ReloadTypes();
    }
}
