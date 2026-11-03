using System.Security.Claims;
using System.Threading.RateLimiting;

namespace EyeCareAPI.Extensions
{
    public static class RateLimiterExtensions
    {
        public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                
                options.AddPolicy("cache-test", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(partitionKey:
                httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions {
                        PermitLimit = 3,
                        Window = TimeSpan.FromSeconds(10),
                        QueueLimit = 0,
                        AutoReplenishment = true }));
                
                options.AddPolicy("user-limit", httpContext =>
            {
                    var userId = httpContext.User
                                    .FindFirst(
                                  ClaimTypes.NameIdentifier)?.Value;

                      var partitionKey = !string.IsNullOrWhiteSpace(userId)
                               ? $"user:{userId}"
                          : $"unauthenticated:{httpContext.Connection.RemoteIpAddress}";

                          return RateLimitPartition.GetFixedWindowLimiter(
                                partitionKey,
                              factory: _ => new FixedWindowRateLimiterOptions
                                    {
                                        PermitLimit = 5,
                                        Window = TimeSpan.FromSeconds(20),
                                         QueueLimit = 0,
                                       AutoReplenishment = true
                                     });
                           });

                options.AddPolicy("token-bucket-test", httpContext =>
                RateLimitPartition.GetTokenBucketLimiter(
                partitionKey:
                  httpContext.Connection.RemoteIpAddress?.ToString()?? "unknown",
                  factory: _ => new TokenBucketRateLimiterOptions
                         {
                             TokenLimit = 5,
                             TokensPerPeriod = 1,
                             ReplenishmentPeriod = TimeSpan.FromSeconds(2),
                             QueueLimit = 0,
                             AutoReplenishment = true
                         }));

                options.AddPolicy("sliding-window-test", httpContext =>
                RateLimitPartition.GetSlidingWindowLimiter(
                   partitionKey:
                                httpContext.Connection.RemoteIpAddress?.ToString()?? "unknown",
                   factory: _ => new SlidingWindowRateLimiterOptions
                   {
                         PermitLimit = 5,
                         Window = TimeSpan.FromSeconds(10),
                         SegmentsPerWindow = 2,
                         QueueLimit = 0,
                         AutoReplenishment = true
                   }));

                options.AddPolicy("concurrency-test", httpContext =>
                RateLimitPartition.GetConcurrencyLimiter(
                  partitionKey:
                               httpContext.Connection.RemoteIpAddress?.ToString()?? "unknown",
                  factory: _ => new ConcurrencyLimiterOptions
                  {
                       PermitLimit = 1,
                       QueueLimit = 0,
                       QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                  }));

                options.AddPolicy("login-limit", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                   partitionKey:
                            httpContext.Connection.RemoteIpAddress?.ToString()?? "unknown",

                   factory: _ => new FixedWindowRateLimiterOptions
                   {
                            PermitLimit = 10,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                    }));


            });

            return services;
        }
    }
}
