using System.Security.Claims;
using EyeCare.Application.Abstractions.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace EyeCare.Infrastructure.Authorization.Permissions;

public sealed class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionService _permissionService;

    public PermissionAuthorizationHandler(
        IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
            return;

        var roles =
            context.User
                .FindAll(ClaimTypes.Role)
                .Select(x => x.Value)
                .ToList();

        foreach (var role in roles)
        {
            var hasPermission =
                await _permissionService
                    .RoleHasPermissionAsync(
                        role,
                        requirement.Permission);

            if (hasPermission)
            {
                context.Succeed(requirement);
                return;
            }
        }
    }
}