using Microsoft.AspNetCore.Authorization;

namespace EyeCare.Infrastructure.Authorization.Permissions;

public sealed class PermissionRequirement
    : IAuthorizationRequirement
{
    public PermissionRequirement(
        string permission)
    {
        Permission = permission;
    }

    public string Permission { get; }
}