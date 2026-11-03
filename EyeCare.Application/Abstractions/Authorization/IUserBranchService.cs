namespace EyeCare.Application.Abstractions.Authorization;

public interface IUserBranchService
{
    Task AssignUserToBranchAsync(
        string userId,
        Guid branchId,
        CancellationToken cancellationToken = default);

    Task<bool> IsUserAssignedToBranchAsync(
        string userId,
        Guid branchId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Guid>> GetUserBranchIdsAsync(
        string userId,
        CancellationToken cancellationToken = default);
}