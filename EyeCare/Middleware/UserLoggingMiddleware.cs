using Serilog.Context;
using System.Security.Claims;

namespace EyeCareAPI.Middleware;

public sealed class UserLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public UserLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var userId = context.User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            await _next(context);
            return;
        }

        using (LogContext.PushProperty("UserId", userId))
        {
            await _next(context);
        }
    }
}
