using EyeCare.Application.Abstractions.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EyeCare.Infrastructure.Caching;

public sealed class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCacheService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 1. استخدام GetAsync بدلاً من GetStringAsync لجلب البيانات كـ byte[]
            var cachedBytes = await _cache.GetAsync(key, cancellationToken);

            if (cachedBytes is null || cachedBytes.Length == 0)
                return default;

            // 2. تحويل الـ byte[] مباشرة إلى الكائن المطلوب
            return JsonSerializer.Deserialize<T>(cachedBytes, JsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
            ex,
            "Redis GET failed for cache key {CacheKey}. Falling back to source.",
            key);
            return default;
        }
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 1. تحويل الكائن مباشرة إلى مصفوفة بايتات UTF-8
            var serializedBytes = JsonSerializer.SerializeToUtf8Bytes(value, JsonOptions);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };

            // 2. استخدام SetAsync بدلاً من SetStringAsync لحفظ الـ byte[]
            await _cache.SetAsync(
                key,
                serializedBytes,
                options,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Redis SET failed for cache key {CacheKey}. Falling back to source.",
                key);
        }
    }

    public Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return _cache.RemoveAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Redis REMOVE failed for cache key {CacheKey}. Falling back to source.",
                key);
            return Task.CompletedTask;
        }
    }
}