using EyeCare.Domain.Entities;
using EyeCare.Domain.Exceptions;

namespace EyeCare.Domain.Tests.Entities;

public class DoctorTests
{
    [Fact]
    public void Constructor_ShouldCreateDoctor_WhenDataIsValid()
    {
        // Arrange
        var firstName = "Ahmed";
        var lastName = "Yasser";
        var phoneNumber = "01012345678";
        var specialty = "Ophthalmology";

        var beforeCreation = DateTime.UtcNow;

        // Act
        var doctor = new Doctor(
            firstName,
            lastName,
            phoneNumber,
            specialty);

        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.NotEqual(Guid.Empty, doctor.Id);

        Assert.Equal(firstName, doctor.FirstName);
        Assert.Equal(lastName, doctor.LastName);
        Assert.Equal(phoneNumber, doctor.PhoneNumber);
        Assert.Equal(specialty, doctor.Specialty);

        Assert.True(doctor.IsActive);

        Assert.InRange(
            doctor.CreatedAt,
            beforeCreation,
            afterCreation);
    }

    [Fact]
    public void Constructor_ShouldGenerateUniqueIds_WhenCreatingDifferentDoctors()
    {
        // Arrange & Act
        var firstDoctor = CreateDoctor();
        var secondDoctor = CreateDoctor();

        // Assert
        Assert.NotEqual(firstDoctor.Id, secondDoctor.Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenFirstNameIsInvalid(
        string? firstName)
    {
        // Act
        var exception = Assert.Throws<DomainException>(() =>
            new Doctor(
                firstName!,
                "Yasser",
                "01012345678",
                "Ophthalmology"));

        // Assert
        Assert.Equal(
            "Doctor first name is required.",
            exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenLastNameIsInvalid(
        string? lastName)
    {
        // Act
        var exception = Assert.Throws<DomainException>(() =>
            new Doctor(
                "Ahmed",
                lastName!,
                "01012345678",
                "Ophthalmology"));

        // Assert
        Assert.Equal(
            "Doctor last name is required.",
            exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenPhoneNumberIsInvalid(
        string? phoneNumber)
    {
        // Act
        var exception = Assert.Throws<DomainException>(() =>
            new Doctor(
                "Ahmed",
                "Yasser",
                phoneNumber!,
                "Ophthalmology"));

        // Assert
        Assert.Equal(
            "Doctor phone number is required.",
            exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenSpecialtyIsInvalid(
        string? specialty)
    {
        // Act
        var exception = Assert.Throws<DomainException>(() =>
            new Doctor(
                "Ahmed",
                "Yasser",
                "01012345678",
                specialty!));

        // Assert
        Assert.Equal(
            "Doctor specialty is required.",
            exception.Message);
    }
    [Fact]
    public void UpdateInformation_ShouldUpdateAllProperties_WhenDataIsValid()
    {
        // Arrange
        var doctor = CreateDoctor();
        var originalId = doctor.Id;
        var originalCreatedAt = doctor.CreatedAt;

        var beforeUpdate = DateTime.UtcNow;

        // Act
        doctor.UpdateInformation(
            "Mohamed",
            "Ali",
            "01112345678",
            "Cardiology");

        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.Equal("Mohamed", doctor.FirstName);
        Assert.Equal("Ali", doctor.LastName);
        Assert.Equal("01112345678", doctor.PhoneNumber);
        Assert.Equal("Cardiology", doctor.Specialty);

        Assert.Equal(originalId, doctor.Id);
        Assert.Equal(originalCreatedAt, doctor.CreatedAt);

        // تأكد أولاً أنها ليست null
        Assert.NotNull(doctor.UpdatedAt);

        // استخدم .Value لجلب القيمة الحقيقية
        Assert.InRange(
            doctor.UpdatedAt.Value,
            beforeUpdate,
            afterUpdate);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateInformation_ShouldThrow_WhenFirstNameIsInvalid(
        string? firstName)
    {
        var doctor = CreateDoctor();

        AssertUpdateFailsWithoutChangingData(
            doctor,
            firstName!,
            "Ali",
            "01112345678",
            "Cardiology",
            "Doctor first name is required.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateInformation_ShouldThrow_WhenLastNameIsInvalid(
        string? lastName)
    {
        var doctor = CreateDoctor();

        AssertUpdateFailsWithoutChangingData(
            doctor,
            "Mohamed",
            lastName!,
            "01112345678",
            "Cardiology",
            "Doctor last name is required.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateInformation_ShouldThrow_WhenPhoneNumberIsInvalid(
        string? phoneNumber)
    {
        var doctor = CreateDoctor();

        AssertUpdateFailsWithoutChangingData(
            doctor,
            "Mohamed",
            "Ali",
            phoneNumber!,
            "Cardiology",
            "Doctor phone number is required.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateInformation_ShouldThrow_WhenSpecialtyIsInvalid(
        string? specialty)
    {
        var doctor = CreateDoctor();

        AssertUpdateFailsWithoutChangingData(
            doctor,
            "Mohamed",
            "Ali",
            "01112345678",
            specialty!,
            "Doctor specialty is required.");
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse_WhenDoctorIsActive()
    {
        // Arrange
        var doctor = CreateDoctor();
        var beforeUpdate = DateTime.UtcNow;

        // Act
        doctor.Deactivate();

        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.False(doctor.IsActive);
        Assert.NotNull(doctor.UpdatedAt);
        Assert.InRange(
            doctor.UpdatedAt.Value,
            beforeUpdate,
            afterUpdate);
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue_WhenDoctorIsInactive()
    {
        // Arrange
        var doctor = CreateDoctor();
        doctor.Deactivate();

        var beforeUpdate = DateTime.UtcNow;

        // Act
        doctor.Activate();

        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.True(doctor.IsActive);
                Assert.NotNull(doctor.UpdatedAt);
        Assert.InRange(
            doctor.UpdatedAt.Value,
            beforeUpdate,
            afterUpdate);
    }

    [Fact]
    public void Activate_ShouldNotChangeUpdatedAt_WhenDoctorIsAlreadyActive()
    {
        // Arrange
        var doctor = CreateDoctor();
        var originalUpdatedAt = doctor.UpdatedAt;

        // Act
        doctor.Activate();

        // Assert
        Assert.True(doctor.IsActive);
        Assert.Equal(originalUpdatedAt, doctor.UpdatedAt);
    }

    [Fact]
    public void Deactivate_ShouldNotChangeUpdatedAt_WhenDoctorIsAlreadyInactive()
    {
        // Arrange
        var doctor = CreateDoctor();
        doctor.Deactivate();

        var originalUpdatedAt = doctor.UpdatedAt;

        // Act
        doctor.Deactivate();

        // Assert
        Assert.False(doctor.IsActive);
        Assert.Equal(originalUpdatedAt, doctor.UpdatedAt);
    }

    [Fact]
    public void AssignToBranch_ShouldAddDoctorBranch_WhenDataIsValid()
    {
        // Arrange
        var doctor = CreateDoctor();

        var doctorBranch = new DoctorBranch(
            doctor.Id,
            Guid.NewGuid());

        var beforeUpdate = DateTime.UtcNow;

        // Act
        doctor.AssignToBranch(doctorBranch);

        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.Single(doctor.DoctorBranches);
        Assert.Contains(doctorBranch, doctor.DoctorBranches);

        Assert.Equal(doctor.Id, doctorBranch.DoctorId);
        Assert.NotNull(doctor.UpdatedAt);
        Assert.InRange(
            doctor.UpdatedAt.Value,
            beforeUpdate,
            afterUpdate);
    }

    [Fact]
    public void AssignToBranch_ShouldThrow_WhenDoctorBranchIsNull()
    {
        // Arrange
        var doctor = CreateDoctor();
        var originalUpdatedAt = doctor.UpdatedAt;

        // Act
        var exception = Assert.Throws<DomainException>(() =>
            doctor.AssignToBranch(null!));

        // Assert
        Assert.Equal(
            "Doctor branch is required.",
            exception.Message);

        Assert.Empty(doctor.DoctorBranches);
        Assert.Equal(originalUpdatedAt, doctor.UpdatedAt);
    }

    [Fact]
    public void AssignToBranch_ShouldThrow_WhenDoctorBranchBelongsToAnotherDoctor()
    {
        // Arrange
        var doctor = CreateDoctor();

        var doctorBranch = new DoctorBranch(
            Guid.NewGuid(),
            Guid.NewGuid());

        var originalUpdatedAt = doctor.UpdatedAt;

        // Act
        var exception = Assert.Throws<DomainException>(() =>
            doctor.AssignToBranch(doctorBranch));

        // Assert
        Assert.Equal(
            "Doctor branch does not belong to this doctor.",
            exception.Message);

        Assert.Empty(doctor.DoctorBranches);
        Assert.Equal(originalUpdatedAt, doctor.UpdatedAt);
    }

    [Fact]
    public void AssignToBranch_ShouldAllowDifferentBranches()
    {
        // Arrange
        var doctor = CreateDoctor();

        var firstDoctorBranch = new DoctorBranch(
            doctor.Id,
            Guid.NewGuid());

        var secondDoctorBranch = new DoctorBranch(
            doctor.Id,
            Guid.NewGuid());

        // Act
        doctor.AssignToBranch(firstDoctorBranch);
        doctor.AssignToBranch(secondDoctorBranch);

        // Assert
        Assert.Equal(2, doctor.DoctorBranches.Count);

        Assert.Contains(firstDoctorBranch, doctor.DoctorBranches);
        Assert.Contains(secondDoctorBranch, doctor.DoctorBranches);
    }

    [Fact]
    public void AssignToBranch_ShouldThrow_WhenDoctorIsAlreadyAssignedToActiveBranch()
    {
        // Arrange
        var doctor = CreateDoctor();

        var doctorBranch = new DoctorBranch(
            doctor.Id,
            Guid.NewGuid());

        doctor.AssignToBranch(doctorBranch);

        var originalUpdatedAt = doctor.UpdatedAt;

        var duplicateDoctorBranch = new DoctorBranch(
            doctor.Id,
            doctorBranch.BranchId);

        // Act
        var exception = Assert.Throws<DomainException>(() =>
            doctor.AssignToBranch(duplicateDoctorBranch));

        // Assert
        Assert.Equal(
            "Doctor is already assigned to this branch.",
            exception.Message);

        Assert.Single(doctor.DoctorBranches);
        Assert.Contains(doctorBranch, doctor.DoctorBranches);
        Assert.Equal(originalUpdatedAt, doctor.UpdatedAt);
    }

    [Fact]
    public void AssignToBranch_ShouldReactivateExistingRelationship_WhenRelationshipIsInactive()
    {
        // Arrange
        var doctor = CreateDoctor();

        var doctorBranch = new DoctorBranch(
            doctor.Id,
            Guid.NewGuid());

        doctor.AssignToBranch(doctorBranch);
        doctorBranch.Deactivate();

        var originalRelationshipId = doctorBranch.Id;
        var beforeUpdate = DateTime.UtcNow;

        var duplicateDoctorBranch = new DoctorBranch(
            doctor.Id,
            doctorBranch.BranchId);

        // Act
        doctor.AssignToBranch(duplicateDoctorBranch);

        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.True(doctorBranch.IsActive);

        Assert.Single(doctor.DoctorBranches);
        Assert.Contains(doctorBranch, doctor.DoctorBranches);

        Assert.Equal(originalRelationshipId, doctorBranch.Id);

        Assert.DoesNotContain(
            duplicateDoctorBranch,
            doctor.DoctorBranches);
        Assert.NotNull(doctor.UpdatedAt);
        Assert.InRange(
            doctor.UpdatedAt.Value,
            beforeUpdate,
            afterUpdate);

        Assert.InRange(
            doctorBranch.UpdatedAt.Value,
            beforeUpdate,
            afterUpdate);
    }

    [Fact]
    public void AssignToBranch_ShouldNotChangeUpdatedAt_WhenDuplicateAssignmentFails()
    {
        // Arrange
        var doctor = CreateDoctor();

        var doctorBranch = new DoctorBranch(
            doctor.Id,
            Guid.NewGuid());

        doctor.AssignToBranch(doctorBranch);

        var originalUpdatedAt = doctor.UpdatedAt;

        var duplicateDoctorBranch = new DoctorBranch(
            doctor.Id,
            doctorBranch.BranchId);

        // Act
        Assert.Throws<DomainException>(() =>
            doctor.AssignToBranch(duplicateDoctorBranch));

        // Assert
        Assert.Equal(originalUpdatedAt, doctor.UpdatedAt);
        Assert.Single(doctor.DoctorBranches);
    }





    private static void AssertUpdateFailsWithoutChangingData(
        Doctor doctor,
        string firstName,
        string lastName,
        string phoneNumber,
        string specialty,
        string expectedMessage)
    {
        // Arrange
        var originalFirstName = doctor.FirstName;
        var originalLastName = doctor.LastName;
        var originalPhoneNumber = doctor.PhoneNumber;
        var originalSpecialty = doctor.Specialty;
        var originalId = doctor.Id;
        var originalCreatedAt = doctor.CreatedAt;
        var originalUpdatedAt = doctor.UpdatedAt;

        // Act
        var exception = Assert.Throws<DomainException>(() =>
            doctor.UpdateInformation(
                firstName,
                lastName,
                phoneNumber,
                specialty));

        // Assert
        Assert.Equal(expectedMessage, exception.Message);

        Assert.Equal(originalFirstName, doctor.FirstName);
        Assert.Equal(originalLastName, doctor.LastName);
        Assert.Equal(originalPhoneNumber, doctor.PhoneNumber);
        Assert.Equal(originalSpecialty, doctor.Specialty);

        Assert.Equal(originalId, doctor.Id);
        Assert.Equal(originalCreatedAt, doctor.CreatedAt);
        Assert.Equal(originalUpdatedAt, doctor.UpdatedAt);
    }


    private static Doctor CreateDoctor()
    {
        return new Doctor(
            "Ahmed",
            "Yasser",
            "01012345678",
            "Ophthalmology");
    }
}

