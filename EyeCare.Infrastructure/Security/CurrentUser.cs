using System.Security.Claims;
using EyeCare.Application.Abstractions.Security;
using Microsoft.AspNetCore.Http;


namespace EyeCare.Infrastructure.Security;

public sealed class CurrentUser
    : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor =
            httpContextAccessor;
    }

    public string? UserId =>
        _httpContextAccessor
            .HttpContext?
            .User?
            .FindFirstValue(
                ClaimTypes.NameIdentifier);
}
