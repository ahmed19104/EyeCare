using EyeCare.Application.Abstractions.Authorization;
using EyeCare.Application.Abstractions.Identity;
using EyeCare.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EyeCare.Infrastructure.Authorization;

public static class PermissionSeeder
{
    public static async Task SeedAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        using var scope =
            serviceProvider.CreateScope();

        var roleManager =
            scope.ServiceProvider
                .GetRequiredService<
                    RoleManager<IdentityRole>>();

        var context =
            scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

        var permissionsByRole =
            new Dictionary<string, string[]>
            {
                [Roles.Admin] =
                [
                    .. Application.Abstractions.Authorization.Permissions.All
                ],

                [Roles.BranchManager] =
                [
                        Application.Abstractions.Authorization.Permissions.Patients.Read,
                        Application.Abstractions.Authorization.Permissions.Patients.Create,
                        Application.Abstractions.Authorization.Permissions.Patients.Update,

                                Application.Abstractions.Authorization.Permissions.Appointments.Read,
                        Application.Abstractions.Authorization.Permissions.Appointments.Create,
                    Application.Abstractions.Authorization.Permissions.Appointments.Update,
                    Application.Abstractions.Authorization.Permissions.Appointments.Cancel,

                    Application.Abstractions.Authorization.Permissions.MedicalRecords.Read,

                    Application.Abstractions.Authorization.Permissions.Billing.Read,

                    Application.Abstractions.Authorization.Permissions.Doctors.Read,
                    Application.Abstractions.Authorization.Permissions.Doctors.ManageSchedule
                ],

                [Roles.Receptionist] =
                [
                    Application.Abstractions.Authorization.Permissions.Patients.Read,
                    Application.Abstractions.Authorization.Permissions.Patients.Create,
                    Application.Abstractions.Authorization.Permissions.Patients.Update,

                    Application.Abstractions.Authorization.Permissions.Appointments.Read,
                    Application.Abstractions.Authorization.Permissions.Appointments.Create,
                    Application.Abstractions.Authorization.Permissions.Appointments.Update,
                    Application.Abstractions.Authorization.Permissions.Appointments.Cancel
                ],

                [Roles.Doctor] =
                [
                    Application.Abstractions.Authorization.Permissions.Patients.Read,

                    Application.Abstractions.Authorization.Permissions.Appointments.Read,
                    Application.Abstractions.Authorization.Permissions.Appointments.Update,

                    Application.Abstractions.Authorization.Permissions.MedicalRecords.Read,
                    Application.Abstractions.Authorization.Permissions.MedicalRecords.Create,
                    Application.Abstractions.Authorization.Permissions.MedicalRecords.Update
                ],

                [Roles.Accountant] =
                [
                    Application.Abstractions.Authorization.Permissions.Patients.Read,

                    Application.Abstractions.Authorization.Permissions.Appointments.Read,

                    Application.Abstractions.Authorization.Permissions.Billing.Read,
                    Application.Abstractions.Authorization.Permissions.Billing.Create,
                    Application.Abstractions.Authorization.Permissions.Billing.Refund
                ]
            };

        foreach (var rolePermissions in permissionsByRole)
        {
            var role =
                await roleManager.FindByNameAsync(
                    rolePermissions.Key);

            if (role is null)
                continue;

            foreach (var permission in rolePermissions.Value)
            {
                var exists =
                    await context.RolePermissions
                        .AnyAsync(
                            x => x.RoleId == role.Id &&
                                 x.Permission == permission,
                            cancellationToken);

                if (exists)
                    continue;

                context.RolePermissions.Add(
                    new Domain.Entities.RolePermission(
                        role.Id,
                        permission));
            }
        }

        await context.SaveChangesAsync(
            cancellationToken);
    }
}