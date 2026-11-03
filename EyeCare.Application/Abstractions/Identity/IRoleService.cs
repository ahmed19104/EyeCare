namespace EyeCare.Application.Abstractions.Identity;

public interface IRoleService
{
    Task EnsureRolesExistAsync(
        CancellationToken cancellationToken = default);

    Task AssignRoleAsync(
        string userId,
        string role,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetUserRolesAsync(
        string userId,
        CancellationToken cancellationToken = default);
}
