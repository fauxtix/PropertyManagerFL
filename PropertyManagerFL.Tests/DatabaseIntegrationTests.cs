using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using PropertyManagerFL.Infrastructure.Context;
using PropertyManagerFL.Infrastructure.Repositories;

namespace PropertyManagerFL.Tests;

/// <summary>
/// Read-only checks against a scratch copy of PropertyManagerDB.
/// Set PMFL_TEST_CONNECTION to the copy's connection string to enable them;
/// they are no-ops by default so CI and public clones never need tenant data.
/// </summary>
public class DatabaseIntegrationTests
{
    private static string? GetConnectionString()
    {
        var value = Environment.GetEnvironmentVariable("PMFL_TEST_CONNECTION");
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private static LookupTableRepository CreateRepository(string connectionString)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:PMConnection"] = connectionString
            })
            .Build();

        return new LookupTableRepository(new DapperContext(configuration),
            NullLogger<LookupTableRepository>.Instance);
    }

    [Fact]
    public async Task LookupTableRepository_ReadsValidIdentifier()
    {
        var connectionString = GetConnectionString();
        if (connectionString is null)
        {
            // No scratch database configured: treat as a no-op so CI/public clones stay green.
            return;
        }

        var repository = CreateRepository(connectionString);
        var rows = await repository.GetLookupTableData("TipoRecebimento");

        Assert.NotNull(rows);
    }

    [Fact]
    public async Task LookupTableRepository_RejectsInvalidIdentifier()
    {
        var connectionString = GetConnectionString();
        if (connectionString is null)
        {
            // No scratch database configured: treat as a no-op so CI/public clones stay green.
            return;
        }

        var repository = CreateRepository(connectionString);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            repository.GetLookupTableData("Tipo.Recebimento"));
    }

    [Fact]
    public void Database_IsReachableAndReadOnlyQueryWorks()
    {
        var connectionString = GetConnectionString();
        if (connectionString is null)
        {
            // No scratch database configured: treat as a no-op so CI/public clones stay green.
            return;
        }

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:PMConnection"] = connectionString
            })
            .Build();

        using var connection = new DapperContext(configuration).CreateConnection();
        connection.Open();
        var result = connection.QueryFirst<int>("SELECT 1");

        Assert.Equal(1, result);
    }
}
