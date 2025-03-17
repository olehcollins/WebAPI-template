using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebAPI;

public sealed class CustomHealthChecks(IConfiguration configuration) : IHealthCheck
{
    private readonly string? _connectionString
        = configuration.GetConnectionString("DefaultConnection");


    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new())
    {
        var healthCheckData = new Dictionary<string, object>();
        var errors = new List<string>();

        var isDatabaseHealthy = await CheckDatabaseConnectionAsync();
        healthCheckData["Database"] = isDatabaseHealthy ? "Healthy" : "Failed";
        if (!isDatabaseHealthy) errors.Add("Database connection failed.");

        return errors.Count switch
        {
            0 => HealthCheckResult.Healthy(
                "All services are healthy.",
                healthCheckData
            ),
            1 => HealthCheckResult.Degraded(
                $"Some services are slow or degraded: {string.Join(", ", errors)}",
                data: healthCheckData
            ),
            _ => HealthCheckResult.Unhealthy(
                $"Some services are slow or degraded: {string.Join(", ", errors)}",
                data: healthCheckData
            )
        };
    }

    private async Task<bool> CheckDatabaseConnectionAsync()
    {
        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(); // Try to open connection
            Console.WriteLine(connection.State == ConnectionState.Open
                ? "✅ Connected to SQLite database!"
                : "Connection to SQLite database failed");
            return connection.State == ConnectionState.Open;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database connection failed: {ex.Message}");
            return false;
        }
    }
}