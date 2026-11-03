using EyeCare.Application.Abstractions.Identity;
using EyeCare.Application.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace EyeCare.Infrastructure.Identity;

public sealed class RoleService : IRoleService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleService(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task EnsureRolesExistAsync(
        CancellationToken cancellationToken = default)
    {
        foreach (var role in Roles.All)
        {
            if (await _roleManager.RoleExistsAsync(role))
                continue;

            var result =
                await _roleManager.CreateAsync(
                    new IdentityRole(role));

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(x => x.Description));

                throw new ConflictException(
                    $"Could not create role '{role}': {errors}");
            }
        }
    }

    public async Task AssignRoleAsync(
        string userId,
        string role,
        CancellationToken cancellationToken = default)
    {
        if (!Roles.All.Contains(role))
        {
            throw new ConflictException(
                $"Role '{role}' is not supported.");
        }

        var user =
            await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            throw new NotFoundException(
                "User",
                Guid.TryParse(userId, out var id)
                    ? id
                    : Guid.Empty);
        }

        if (!await _roleManager.RoleExistsAsync(role))
        {
            throw new NotFoundException(
                "Role",
                Guid.Empty);
        }

        if (await _userManager.IsInRoleAsync(user, role))
        {
            throw new ConflictException(
                $"User is already assigned to role '{role}'.");
        }

        var result =
            await _userManager.AddToRoleAsync(
                user,
                role);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                "; ",
                result.Errors.Select(x => x.Description));

            throw new ConflictException(errors);
        }
    }

    public async Task<IReadOnlyList<string>> GetUserRolesAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var user =
            await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            throw new NotFoundException(
                "User",
                Guid.TryParse(userId, out var id)
                    ? id
                    : Guid.Empty);
        }

        var roles = await _userManager.GetRolesAsync(user);
        return roles.ToList();
    }
}