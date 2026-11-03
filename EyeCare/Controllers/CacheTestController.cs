using EyeCare.Application.Abstractions.Caching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EyeCareAPI.Controllers;

[ApiController]
[Route("api/cache-test")]
public sealed class CacheTestController : ControllerBase
{
    private readonly ICacheService _cache;

    public CacheTestController(ICacheService cache)
    {
        _cache = cache;
    }

    [HttpGet]
    //[Authorize]
    //[EnableRateLimiting("cache-test")]
    //[EnableRateLimiting("user-limit")]
    //[EnableRateLimiting("token-bucket-test")]
    [EnableRateLimiting("sliding-window-test")]

    public async Task<IActionResult> Get(
        CancellationToken cancellationToken)
    {
        const string key = "test:hello";

        var cachedValue =
            await _cache.GetAsync<string>(
                key,
                cancellationToken);

        if (cachedValue is not null)
        {
            return Ok(new
            {
                source = "redis",
                value = cachedValue
            });
        }

        var value = $"Hello at {DateTime.UtcNow:O}";

        await _cache.SetAsync(
            key,
            value,
            TimeSpan.FromMinutes(5),
            cancellationToken);

        return Ok(new
        {
            source = "database",
            value
        });


    }
    [EnableRateLimiting("concurrency-test")]
    [HttpGet("slow")]
    public async Task<IActionResult> Slow()
    {
        await Task.Delay(TimeSpan.FromSeconds(20));

        return Ok(new
        {
            Message = "Finished"
        });
    }
}
