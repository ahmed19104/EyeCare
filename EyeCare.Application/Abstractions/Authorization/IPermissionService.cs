namespace EyeCare.Application.Abstractions.Authorization;

public interface IPermissionService
{
    Task AssignPermissionToRoleAsync(
        string role,
        string permission,
        CancellationToken cancellationToken = default);

    Task<bool> RoleHasPermissionAsync(
        string role,
        string permission,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetRolePermissionsAsync(
        string role,
        CancellationToken cancellationToken = default);
}