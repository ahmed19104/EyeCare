namespace EyeCare.Application.Abstractions.Authorization;

public interface IBranchAuthorizationService
{
    Task EnsureCanAccessBranchAsync(
        Guid branchId,
        CancellationToken cancellationToken = default);
}
