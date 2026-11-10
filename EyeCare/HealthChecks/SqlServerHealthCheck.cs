
using EyeCare.Infrastructure.Persistence;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EyeCareAPI.HealthChecks;

public sealed class SqlServerHealthCheck : IHealthCheck
{
    private readonly AppDbContext _dbContext;

    public SqlServerHealthCheck(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var canConnect = await _dbContext.Database
            .CanConnectAsync(cancellationToken);

        if (!canConnect)
        {
            return HealthCheckResult.Unhealthy(
                "SQL Server is unavailable.");
        }

        return HealthCheckResult.Healthy(
            "SQL Server is available.");
    }
}

