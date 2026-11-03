using EyeCare.Application.Abstractions.Authorization;
using EyeCare.Application.Abstractions.Security;
using EyeCare.Application.Exceptions;

namespace EyeCare.Infrastructure.Authorization;

public sealed class BranchAuthorizationService
    : IBranchAuthorizationService
{
    private readonly ICurrentUser _currentUser;
    private readonly IUserBranchService _userBranchService;

    public BranchAuthorizationService(
        ICurrentUser currentUser,
        IUserBranchService userBranchService)
    {
        _currentUser = currentUser;
        _userBranchService = userBranchService;
    }

    public async Task EnsureCanAccessBranchAsync(
        Guid branchId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_currentUser.UserId))
        {
            throw new ForbiddenException(
                "You are not assigned to a user.");
        }

        var hasAccess =
            await _userBranchService
                .IsUserAssignedToBranchAsync(
                    _currentUser.UserId,
                    branchId,
                    cancellationToken);

        if (!hasAccess)
        {
            throw new ForbiddenException(
                "You do not have access to this branch.");
        }
    }
}