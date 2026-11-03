
using EyeCare.Domain.Entities;
using EyeCare.Domain.Exceptions;
using Xunit;

namespace EyeCare.Domain.Tests.Entities;

public class UserBranchTests
{
    // =========================
    // Constructor Tests
    // =========================

    [Fact]
    public void Constructor_ShouldCreateUserBranch_WithValidData()
    {
        // Arrange
        var userId = "user-123";
        var branchId = Guid.NewGuid();

        // Act
        var userBranch = new UserBranch(userId, branchId);

        // Assert
        Assert.Equal(userId, userBranch.UserId);
        Assert.Equal(branchId, userBranch.BranchId);
        Assert.True(userBranch.IsActive);
    }

    [Fact]
    public void Constructor_ShouldGenerateUniqueId()
    {
        var first = CreateUserBranch();
        var second = CreateUserBranch();

        Assert.NotEqual(Guid.Empty, first.Id);
        Assert.NotEqual(Guid.Empty, second.Id);
        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void Constructor_ShouldSetCreatedAtInUtc()
    {
        var before = DateTime.UtcNow;

        var userBranch = CreateUserBranch();

        var after = DateTime.UtcNow;

        Assert.InRange(userBranch.CreatedAt, before, after);
        Assert.Equal(DateTimeKind.Utc, userBranch.CreatedAt.Kind);
    }

    [Fact]
    public void Constructor_ShouldSetIsActiveToTrue()
    {
        var userBranch = CreateUserBranch();

        Assert.True(userBranch.IsActive);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Constructor_ShouldThrow_WhenUserIdIsNullOrWhitespace(
        string? userId)
    {
        var exception = Assert.Throws<DomainException>(
            () => new UserBranch(userId!, Guid.NewGuid()));

        Assert.Equal("User is required.", exception.Message);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenBranchIdIsEmpty()
    {
        var exception = Assert.Throws<DomainException>(
            () => new UserBranch("user-123", Guid.Empty));

        Assert.Equal("Branch is required.", exception.Message);
    }

    [Fact]
    public void Constructor_ShouldPreserveUserIdExactly()
    {
        var userId = "  user-123  ";

        var userBranch = new UserBranch(
            userId,
            Guid.NewGuid());

        Assert.Equal(userId, userBranch.UserId);
    }

    // =========================
    // Activate Tests
    // =========================

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue_WhenCurrentlyInactive()
    {
        var userBranch = CreateUserBranch();
        userBranch.Deactivate();

        userBranch.Activate();

        Assert.True(userBranch.IsActive);
    }

    [Fact]
    public void Activate_ShouldUpdateUpdatedAt_WhenSuccessful()
    {
        var userBranch = CreateUserBranch();
        userBranch.Deactivate();

        var before = DateTime.UtcNow;

        userBranch.Activate();

        var after = DateTime.UtcNow;

        Assert.NotNull(userBranch.UpdatedAt);
        Assert.InRange(userBranch.UpdatedAt!.Value, before, after);
        Assert.Equal(DateTimeKind.Utc, userBranch.UpdatedAt.Value.Kind);
    }

    [Fact]
    public void Activate_ShouldThrow_WhenAlreadyActive()
    {
        var userBranch = CreateUserBranch();

        var exception = Assert.Throws<DomainException>(
            () => userBranch.Activate());

        Assert.Equal(
            "User is already assigned to this branch.",
            exception.Message);
    }

    [Fact]
    public void Activate_ShouldPreserveState_WhenAlreadyActive()
    {
        var userBranch = CreateUserBranch();
        var originalUpdatedAt = userBranch.UpdatedAt;

        Assert.Throws<DomainException>(
            () => userBranch.Activate());

        Assert.True(userBranch.IsActive);
        Assert.Equal(originalUpdatedAt, userBranch.UpdatedAt);
    }

    // =========================
    // Deactivate Tests
    // =========================

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse_WhenCurrentlyActive()
    {
        var userBranch = CreateUserBranch();

        userBranch.Deactivate();

        Assert.False(userBranch.IsActive);
    }

    [Fact]
    public void Deactivate_ShouldUpdateUpdatedAt_WhenSuccessful()
    {
        var userBranch = CreateUserBranch();

        var before = DateTime.UtcNow;

        userBranch.Deactivate();

        var after = DateTime.UtcNow;

        Assert.NotNull(userBranch.UpdatedAt);
        Assert.InRange(userBranch.UpdatedAt!.Value, before, after);
        Assert.Equal(DateTimeKind.Utc, userBranch.UpdatedAt.Value.Kind);
    }

    [Fact]
    public void Deactivate_ShouldThrow_WhenAlreadyInactive()
    {
        var userBranch = CreateUserBranch();
        userBranch.Deactivate();

        var exception = Assert.Throws<DomainException>(
            () => userBranch.Deactivate());

        Assert.Equal(
            "User is already inactive in this branch.",
            exception.Message);
    }

    [Fact]
    public void Deactivate_ShouldPreserveState_WhenAlreadyInactive()
    {
        var userBranch = CreateUserBranch();
        userBranch.Deactivate();

        var originalUpdatedAt = userBranch.UpdatedAt;

        Assert.Throws<DomainException>(
            () => userBranch.Deactivate());

        Assert.False(userBranch.IsActive);
        Assert.Equal(originalUpdatedAt, userBranch.UpdatedAt);
    }

    // =========================
    // Lifecycle Tests
    // =========================

    [Fact]
    public void DeactivateAndActivate_ShouldAllowReactivation()
    {
        var userBranch = CreateUserBranch();

        userBranch.Deactivate();
        userBranch.Activate();

        Assert.True(userBranch.IsActive);
    }

    [Fact]
    public void MultipleLifecycleTransitions_ShouldMaintainCorrectState()
    {
        var userBranch = CreateUserBranch();

        Assert.True(userBranch.IsActive);

        userBranch.Deactivate();
        Assert.False(userBranch.IsActive);

        userBranch.Activate();
        Assert.True(userBranch.IsActive);

        userBranch.Deactivate();
        Assert.False(userBranch.IsActive);
    }

    [Fact]
    public void LifecycleTransitions_ShouldNotChangeUserOrBranchIds()
    {
        var userId = "user-123";
        var branchId = Guid.NewGuid();

        var userBranch = new UserBranch(userId, branchId);

        userBranch.Deactivate();
        userBranch.Activate();

        Assert.Equal(userId, userBranch.UserId);
        Assert.Equal(branchId, userBranch.BranchId);
    }

    // =========================
    // Helpers
    // =========================

    private static UserBranch CreateUserBranch()
    {
        return new UserBranch(
            "user-123",
            Guid.NewGuid());
    }
}
