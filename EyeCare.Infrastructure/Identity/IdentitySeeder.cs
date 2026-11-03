using EyeCare.Application.Abstractions.Identity;
using EyeCare.Infrastructure.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace EyeCare.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        using var scope =
            serviceProvider.CreateScope();

        var roleService =
            scope.ServiceProvider
                .GetRequiredService<IRoleService>();

        await roleService.EnsureRolesExistAsync(
            cancellationToken);

        await PermissionSeeder.SeedAsync(
            serviceProvider,
            cancellationToken);
    }
}