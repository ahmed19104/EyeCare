using EyeCare.Domain.Common;
using EyeCare.Domain.Exceptions;

namespace EyeCare.Domain.Entities;

public class UserBranch : BaseEntity
{
    private UserBranch()
    {
    }

    public UserBranch(
        string userId,
        Guid branchId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new DomainException("User is required.");

        if (branchId == Guid.Empty)
            throw new DomainException("Branch is required.");

        Id = Guid.NewGuid();

        UserId = userId;
        BranchId = branchId;
        IsActive = true;

        CreatedAt = DateTime.UtcNow;
    }

    public string UserId { get; private set; } = null!;

    public Guid BranchId { get; private set; }

    public bool IsActive { get; private set; }

    public Branch Branch { get; private set; } = null!;

    public void Activate()
    {
        if (IsActive)
            throw new DomainException(
                "User is already assigned to this branch.");

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException(
                "User is already inactive in this branch.");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}