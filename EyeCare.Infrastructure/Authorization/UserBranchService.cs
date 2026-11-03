using EyeCare.Application.Abstractions.Authorization;
using EyeCare.Application.Exceptions;
using EyeCare.Domain.Entities;
using EyeCare.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EyeCare.Infrastructure.Authorization;

public sealed class UserBranchService
    : IUserBranchService
{
    private readonly AppDbContext _context;

    public UserBranchService(AppDbContext context)
    {
        _context = context;
    }

    public async Task AssignUserToBranchAsync(
        string userId,
        Guid branchId,
        CancellationToken cancellationToken = default)
    {
        var branchExists =
            await _context.Branches
                .AnyAsync(
                    x => x.Id == branchId &&
                         x.IsActive,
                    cancellationToken);

        if (!branchExists)
        {
            throw new NotFoundException(
                "Active branch",
                branchId);
        }

        var existing =
            await _context.UserBranches
                .FirstOrDefaultAsync(
                    x => x.UserId == userId &&
                         x.BranchId == branchId,
                    cancellationToken);

        if (existing is not null)
        {
            if (existing.IsActive)
            {
                throw new ConflictException(
                    "User is already assigned to this branch.");
            }

            existing.Activate();

            await _context.SaveChangesAsync(
                cancellationToken);

            return;
        }

        var userBranch =
            new UserBranch(
                userId,
                branchId);

        await _context.UserBranches.AddAsync(
            userBranch,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<bool> IsUserAssignedToBranchAsync(
        string userId,
        Guid branchId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserBranches
            .AnyAsync(
                x => x.UserId == userId &&
                     x.BranchId == branchId &&
                     x.IsActive,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Guid>> GetUserBranchIdsAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserBranches
            .Where(
                x => x.UserId == userId &&
                     x.IsActive)
            .Select(x => x.BranchId)
            .ToListAsync(cancellationToken);
    }
}