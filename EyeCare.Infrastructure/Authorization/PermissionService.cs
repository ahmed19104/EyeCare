using EyeCare.Application.Abstractions.Authorization;
using EyeCare.Application.Exceptions;
using EyeCare.Domain.Entities;
using EyeCare.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EyeCare.Infrastructure.Authorization;

public sealed class PermissionService
    : IPermissionService
{
    private readonly AppDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;

    public PermissionService(
        AppDbContext context,
        RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _roleManager = roleManager;
    }

    public async Task AssignPermissionToRoleAsync(
        string role,
        string permission,
        CancellationToken cancellationToken = default)
    {
        if (!Application.Abstractions.Authorization.Permissions.All.Contains(permission))
        {
            throw new ConflictException(
                $"Permission '{permission}' is not supported.");
        }

        var identityRole =
            await _roleManager.FindByNameAsync(role);

        if (identityRole is null)
        {
            throw new NotFoundException(
                "Role",
                Guid.Empty);
        }

        var exists =
            await _context.RolePermissions
                .AnyAsync(
                    x => x.RoleId == identityRole.Id &&
                         x.Permission == permission,
                    cancellationToken);

        if (exists)
        {
            throw new ConflictException(
                $"Role '{role}' already has permission '{permission}'.");
        }

        var rolePermission =
            new RolePermission(
                identityRole.Id,
                permission);

        await _context.RolePermissions.AddAsync(
            rolePermission,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<bool> RoleHasPermissionAsync(
        string role,
        string permission,
        CancellationToken cancellationToken = default)
    {
        var identityRole =
            await _roleManager.FindByNameAsync(role);

        if (identityRole is null)
            return false;

        return await _context.RolePermissions
            .AnyAsync(
                x => x.RoleId == identityRole.Id &&
                     x.Permission == permission,
                cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetRolePermissionsAsync(
        string role,
        CancellationToken cancellationToken = default)
    {
        var identityRole =
            await _roleManager.FindByNameAsync(role);

        if (identityRole is null)
        {
            throw new NotFoundException(
                "Role",
                Guid.Empty);
        }

        return await _context.RolePermissions
            .Where(
                x => x.RoleId == identityRole.Id)
            .Select(x => x.Permission)
            .ToListAsync(cancellationToken);
    }
}