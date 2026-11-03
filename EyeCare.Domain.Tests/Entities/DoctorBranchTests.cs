
using EyeCare.Domain.Entities;
using EyeCare.Domain.Exceptions;

namespace EyeCare.Domain.Tests.Entities;

public class DoctorBranchTests
{
    private static readonly Guid DoctorId = Guid.NewGuid();
    private static readonly Guid BranchId = Guid.NewGuid();

    private static DoctorBranch CreateDoctorBranch()
    {
        return new DoctorBranch(
            DoctorId,
            BranchId);
    }

    // Constructor Tests

    [Fact]
    public void Constructor_ShouldCreateActiveDoctorBranch_WhenDataIsValid()
    {
        // Arrange & Act
        var doctorBranch = CreateDoctorBranch();

        // Assert
        Assert.NotEqual(Guid.Empty, doctorBranch.Id);

        Assert.Equal(DoctorId, doctorBranch.DoctorId);

        Assert.Equal(BranchId, doctorBranch.BranchId);

        Assert.True(doctorBranch.IsActive);

        Assert.NotEqual(default, doctorBranch.CreatedAt);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenDoctorIdIsEmpty()
    {
        // Arrange & Act
        var act = () => new DoctorBranch(
            Guid.Empty,
            BranchId);

        // Assert
        var exception = Assert.Throws<DomainException>(act);

        Assert.Equal(
            "Doctor is required.",
            exception.Message);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenBranchIdIsEmpty()
    {
        // Arrange & Act
        var act = () => new DoctorBranch(
            DoctorId,
            Guid.Empty);

        // Assert
        var exception = Assert.Throws<DomainException>(act);

        Assert.Equal(
            "Branch is required.",
            exception.Message);
    }

    [Fact]
    public void Constructor_ShouldGenerateDifferentIds_WhenCreatingDifferentInstances()
    {
        // Arrange & Act
        var first = CreateDoctorBranch();
        var second = CreateDoctorBranch();

        // Assert
        Assert.NotEqual(first.Id, second.Id);
    }

    // Activate Tests

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue_WhenDoctorBranchIsInactive()
    {
        // Arrange
        var doctorBranch = CreateDoctorBranch();
        doctorBranch.Deactivate();

        // Act
        doctorBranch.Activate();

        // Assert
        Assert.True(doctorBranch.IsActive);
    }

    [Fact]
    public void Activate_ShouldUpdateUpdatedAt_WhenDoctorBranchIsInactive()
    {
        // Arrange
        var doctorBranch = CreateDoctorBranch();
        doctorBranch.Deactivate();

        var previousUpdatedAt = doctorBranch.UpdatedAt;

        // Act
        doctorBranch.Activate();

        // Assert
        Assert.NotEqual(
            previousUpdatedAt,
            doctorBranch.UpdatedAt);

        Assert.NotEqual(
            default,
            doctorBranch.UpdatedAt);
    }

    [Fact]
    public void Activate_ShouldNotUpdateUpdatedAt_WhenDoctorBranchIsAlreadyActive()
    {
        // Arrange
        var doctorBranch = CreateDoctorBranch();

        var previousUpdatedAt = doctorBranch.UpdatedAt;

        // Act
        doctorBranch.Activate();

        // Assert
        Assert.True(doctorBranch.IsActive);

        Assert.Equal(
            previousUpdatedAt,
            doctorBranch.UpdatedAt);
    }

    // Deactivate Tests

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse_WhenDoctorBranchIsActive()
    {
        // Arrange
        var doctorBranch = CreateDoctorBranch();

        // Act
        doctorBranch.Deactivate();

        // Assert
        Assert.False(doctorBranch.IsActive);
    }

    [Fact]
    public void Deactivate_ShouldUpdateUpdatedAt_WhenDoctorBranchIsActive()
    {
        // Arrange
        var doctorBranch = CreateDoctorBranch();

        var previousUpdatedAt = doctorBranch.UpdatedAt;

        // Act
        doctorBranch.Deactivate();

        // Assert
        Assert.NotEqual(
            previousUpdatedAt,
            doctorBranch.UpdatedAt);

        Assert.NotEqual(
            default,
            doctorBranch.UpdatedAt);
    }

    [Fact]
    public void Deactivate_ShouldNotUpdateUpdatedAt_WhenDoctorBranchIsAlreadyInactive()
    {
        // Arrange
        var doctorBranch = CreateDoctorBranch();
        doctorBranch.Deactivate();

        var previousUpdatedAt = doctorBranch.UpdatedAt;

        // Act
        doctorBranch.Deactivate();

        // Assert
        Assert.False(doctorBranch.IsActive);

        Assert.Equal(
            previousUpdatedAt,
            doctorBranch.UpdatedAt);
    }
    [Fact]
    public void activate_ShouldNotUpdateUpdatedAt_WhenDoctorBranchIsAlreadyInactive()
    {
        // Arrange
        var doctorBranch = CreateDoctorBranch();
        doctorBranch.Activate();

        var previousUpdatedAt = doctorBranch.UpdatedAt;

        // Act
        doctorBranch.Activate();

        // Assert
        Assert.True(doctorBranch.IsActive);

        Assert.Equal(
            previousUpdatedAt,
            doctorBranch.UpdatedAt);
    }
}

