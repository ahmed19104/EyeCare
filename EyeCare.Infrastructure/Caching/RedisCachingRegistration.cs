using EyeCare.Application.Abstractions.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EyeCare.Infrastructure.Caching;

public static class RedisCachingRegistration
{
    public static IServiceCollection AddRedisCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("Redis");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "Redis connection string is missing.");

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = connectionString;
            options.InstanceName = "EyeCare:";
        });

        services.AddScoped<ICacheService, RedisCacheService>();

        return services;
    }
}
