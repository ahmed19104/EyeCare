
using EyeCare.Domain.Entities;
using EyeCare.Domain.Exceptions;

namespace EyeCare.Domain.Tests.Entities;

public class BranchTests
{
    private static Branch CreateBranch()
    {
        return new Branch(
            "EyeCare Giza",
            "Giza, Egypt",
            "01012345678");
    }

    // Constructor Tests

    [Fact]
    public void Constructor_ShouldCreateActiveBranch_WhenDataIsValid()
    {
        // Arrange & Act
        var branch = CreateBranch();

        // Assert
        Assert.NotEqual(Guid.Empty, branch.Id);
        Assert.Equal("EyeCare Giza", branch.Name);
        Assert.Equal("Giza, Egypt", branch.Address);
        Assert.Equal("01012345678", branch.PhoneNumber);
        Assert.True(branch.IsActive);
        Assert.NotEqual(default, branch.CreatedAt);
    }

    [Fact]
    public void Constructor_ShouldGenerateDifferentIds_WhenCreatingDifferentBranches()
    {
        // Arrange & Act
        var first = CreateBranch();
        var second = CreateBranch();

        // Assert
        Assert.NotEqual(first.Id, second.Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenNameIsInvalid(string? name)
    {
        // Arrange & Act
        var act = () => new Branch(
            name!,
            "Giza, Egypt",
            "01012345678");

        // Assert
        var exception = Assert.Throws<DomainException>(act);

        Assert.Equal("Branch name is required.", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenAddressIsInvalid(string? address)
    {
        // Arrange & Act
        var act = () => new Branch(
            "EyeCare Giza",
            address!,
            "01012345678");

        // Assert
        var exception = Assert.Throws<DomainException>(act);

        Assert.Equal("Branch address is required.", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenPhoneNumberIsInvalid(string? phoneNumber)
    {
        // Arrange & Act
        var act = () => new Branch(
            "EyeCare Giza",
            "Giza, Egypt",
            phoneNumber!);

        // Assert
        var exception = Assert.Throws<DomainException>(act);

        Assert.Equal("Branch phone number is required.", exception.Message);
    }

    // UpdateInformation Tests

    [Fact]
    public void UpdateInformation_ShouldUpdateAllProperties_WhenDataIsValid()
    {
        // Arrange
        var branch = CreateBranch();

        // Act
        branch.UpdateInformation(
            "EyeCare Cairo",
            "Cairo, Egypt",
            "01198765432");

        // Assert
        Assert.Equal("EyeCare Cairo", branch.Name);
        Assert.Equal("Cairo, Egypt", branch.Address);
        Assert.Equal("01198765432", branch.PhoneNumber);
        Assert.NotEqual(default, branch.UpdatedAt);
    }

    [Fact]
    public void UpdateInformation_ShouldThrow_WhenNameIsInvalid()
    {
        // Arrange
        var branch = CreateBranch();

        var originalName = branch.Name;
        var originalAddress = branch.Address;
        var originalPhone = branch.PhoneNumber;
        var originalUpdatedAt = branch.UpdatedAt;

        // Act
        var act = () => branch.UpdateInformation(
            " ",
            "New Address",
            "01198765432");

        // Assert
        Assert.Throws<DomainException>(act);

        Assert.Equal(originalName, branch.Name);
        Assert.Equal(originalAddress, branch.Address);
        Assert.Equal(originalPhone, branch.PhoneNumber);
        Assert.Equal(originalUpdatedAt, branch.UpdatedAt);
    }

    [Fact]
    public void UpdateInformation_ShouldThrow_WhenAddressIsInvalid()
    {
        // Arrange
        var branch = CreateBranch();

        var originalName = branch.Name;
        var originalAddress = branch.Address;
        var originalPhone = branch.PhoneNumber;
        var originalUpdatedAt = branch.UpdatedAt;

        // Act
        var act = () => branch.UpdateInformation(
            "New Name",
            " ",
            "01198765432");

        // Assert
        Assert.Throws<DomainException>(act);

        Assert.Equal(originalName, branch.Name);
        Assert.Equal(originalAddress, branch.Address);
        Assert.Equal(originalPhone, branch.PhoneNumber);
        Assert.Equal(originalUpdatedAt, branch.UpdatedAt);
    }

    [Fact]
    public void UpdateInformation_ShouldThrow_WhenPhoneNumberIsInvalid()
    {
        // Arrange
        var branch = CreateBranch();

        var originalName = branch.Name;
        var originalAddress = branch.Address;
        var originalPhone = branch.PhoneNumber;
        var originalUpdatedAt = branch.UpdatedAt;

        // Act
        var act = () => branch.UpdateInformation(
            "New Name",
            "New Address",
            " ");

        // Assert
        Assert.Throws<DomainException>(act);

        Assert.Equal(originalName, branch.Name);
        Assert.Equal(originalAddress, branch.Address);
        Assert.Equal(originalPhone, branch.PhoneNumber);
        Assert.Equal(originalUpdatedAt, branch.UpdatedAt);
    }

    // Activate Tests

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue_WhenBranchIsInactive()
    {
        // Arrange
        var branch = CreateBranch();
        branch.Deactivate();

        // Act
        branch.Activate();

        // Assert
        Assert.True(branch.IsActive);
    }

    [Fact]
    public void Activate_ShouldNotUpdateUpdatedAt_WhenBranchIsAlreadyActive()
    {
        // Arrange
        var branch = CreateBranch();
        var originalUpdatedAt = branch.UpdatedAt;

        // Act
        branch.Activate();

        // Assert
        Assert.True(branch.IsActive);
        Assert.Equal(originalUpdatedAt, branch.UpdatedAt);
    }

    // Deactivate Tests

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse_WhenBranchIsActive()
    {
        // Arrange
        var branch = CreateBranch();

        // Act
        branch.Deactivate();

        // Assert
        Assert.False(branch.IsActive);
    }

    [Fact]
    public void Deactivate_ShouldNotUpdateUpdatedAt_WhenBranchIsAlreadyInactive()
    {
        // Arrange
        var branch = CreateBranch();
        branch.Deactivate();

        var originalUpdatedAt = branch.UpdatedAt;

        // Act
        branch.Deactivate();

        // Assert
        Assert.False(branch.IsActive);
        Assert.Equal(originalUpdatedAt, branch.UpdatedAt);
    }

    // DoctorBranches Collection Tests

    [Fact]
    public void DoctorBranches_ShouldInitiallyBeEmpty_WhenBranchIsCreated()
    {
        // Arrange & Act
        var branch = CreateBranch();

        // Assert
        Assert.Empty(branch.DoctorBranches);
    }

    [Fact]
    public void AddDoctor_ShouldAddDoctorBranch_WhenDataIsValid()
    {
        // Arrange
        var branch = CreateBranch();
        var doctorBranch = new DoctorBranch(
            Guid.NewGuid(),
            branch.Id);

        // Act
        branch.AddDoctor(doctorBranch);

        // Assert
        Assert.Single(branch.DoctorBranches);
        Assert.Contains(doctorBranch, branch.DoctorBranches);
    }

    [Fact]
    public void AddDoctor_ShouldThrow_WhenDoctorBranchIsNull()
    {
        // Arrange
        var branch = CreateBranch();

        // Act
        var act = () => branch.AddDoctor(null!);

        // Assert
        var exception = Assert.Throws<DomainException>(act);

        Assert.Equal(
            "Doctor branch is required.",
            exception.Message);

        Assert.Empty(branch.DoctorBranches);
    }

    [Fact]
    public void AddDoctor_ShouldThrow_WhenDoctorIsAlreadyAssigned()
    {
        // Arrange
        var branch = CreateBranch();
        var doctorId = Guid.NewGuid();

        var firstRelationship = new DoctorBranch(
            doctorId,
            branch.Id);

        var duplicateRelationship = new DoctorBranch(
            doctorId,
            branch.Id);

        branch.AddDoctor(firstRelationship);

        // Act
        var act = () => branch.AddDoctor(duplicateRelationship);

        // Assert
        var exception = Assert.Throws<DomainException>(act);

        Assert.Equal(
            "Doctor is already assigned to this branch.",
            exception.Message);

        Assert.Single(branch.DoctorBranches);
        Assert.Contains(firstRelationship, branch.DoctorBranches);
    }

    [Fact]
    public void AddDoctor_ShouldAllowDifferentDoctors_InSameBranch()
    {
        // Arrange
        var branch = CreateBranch();

        var firstDoctor = new DoctorBranch(
            Guid.NewGuid(),
            branch.Id);

        var secondDoctor = new DoctorBranch(
            Guid.NewGuid(),
            branch.Id);

        // Act
        branch.AddDoctor(firstDoctor);
        branch.AddDoctor(secondDoctor);

        // Assert
        Assert.Equal(2, branch.DoctorBranches.Count);
        Assert.Contains(firstDoctor, branch.DoctorBranches);
        Assert.Contains(secondDoctor, branch.DoctorBranches);
    }

    [Fact]
    public void AddDoctor_ShouldThrow_WhenDoctorBranchBelongsToAnotherBranch()
    {
        // Arrange
        var branch = CreateBranch();

        var doctorBranch = new DoctorBranch(
            Guid.NewGuid(),
            Guid.NewGuid());

        // Act
        var act = () => branch.AddDoctor(doctorBranch);

        // Assert
        Assert.Throws<DomainException>(act);

        Assert.Empty(branch.DoctorBranches);
    }

    [Fact]
    public void AddDoctor_ShouldUpdateUpdatedAt_WhenDoctorIsAdded()
    {
        // Arrange
        var branch = CreateBranch();

        var doctorBranch = new DoctorBranch(
            Guid.NewGuid(),
            branch.Id);

        // Act
        branch.AddDoctor(doctorBranch);

        // Assert
        Assert.NotEqual(default, branch.UpdatedAt);
    }

    [Fact]
    public void AddDoctor_ShouldNotChangeUpdatedAt_WhenDoctorIsAlreadyAssigned()
    {
        // Arrange
        var branch = CreateBranch();

        var doctorId = Guid.NewGuid();

        branch.AddDoctor(new DoctorBranch(doctorId, branch.Id));

        var previousUpdatedAt = branch.UpdatedAt;

        // Act
        var act = () => branch.AddDoctor(
            new DoctorBranch(doctorId, branch.Id));

        // Assert
        Assert.Throws<DomainException>(act);

        Assert.Equal(previousUpdatedAt, branch.UpdatedAt);
    }

}

