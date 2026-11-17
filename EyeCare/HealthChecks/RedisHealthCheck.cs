using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EyeCareAPI.HealthChecks;

public sealed class RedisHealthCheck : IHealthCheck
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisHealthCheck> _logger;

    public RedisHealthCheck(
        IDistributedCache cache,
        ILogger<RedisHealthCheck> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var key = $"health-check:{Guid.NewGuid()}";
        var value = Guid.NewGuid().ToString();

        try
        {
            await _cache.SetStringAsync(
                key,
                value,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
                },
                cancellationToken);

            var retrievedValue = await _cache.GetStringAsync(
                key,
                cancellationToken);

            if (retrievedValue != value)
            {
                return HealthCheckResult.Unhealthy(
                    "Redis read/write check failed.");
            }

            return HealthCheckResult.Healthy("Redis is available.");
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        
            catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Redis health check failed.");

            return HealthCheckResult.Unhealthy(
                "Redis is unavailable.",
                ex);
        }
    }
    
}