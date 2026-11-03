
using EyeCare.Domain.Entities;
using EyeCare.Domain.Enums;
using EyeCare.Domain.Exceptions;
using Xunit;

namespace EyeCare.Domain.Tests.Entities;

public class PatientTests
{
    private static Patient CreatePatient(
        string firstName = "Ahmed",
        string lastName = "Yasser",
        DateTime? dateOfBirth = null,
        Gender gender = default,
        string phoneNumber = "01012345678",
        string? email = "ahmed@example.com",
        string? nationalId = "12345678901234")
    {
        return new Patient(
            firstName,
            lastName,
            dateOfBirth ?? new DateTime(2000, 1, 1),
            gender,
            phoneNumber,
            email,
            nationalId);
    }

    [Fact]
    public void Constructor_ShouldCreatePatient_WithValidData()
    {
        var dateOfBirth = new DateTime(2000, 1, 1);
        var before = DateTime.UtcNow;

        var patient = CreatePatient(
            dateOfBirth: dateOfBirth,
            email: "patient@example.com",
            nationalId: "12345678901234");

        var after = DateTime.UtcNow;

        Assert.NotEqual(Guid.Empty, patient.Id);
        Assert.Equal("Ahmed", patient.FirstName);
        Assert.Equal("Yasser", patient.LastName);
        Assert.Equal(dateOfBirth, patient.DateOfBirth);
        Assert.Equal("01012345678", patient.PhoneNumber);
        Assert.Equal("patient@example.com", patient.Email);
        Assert.Equal("12345678901234", patient.NationalId);
        Assert.True(patient.IsActive);
        Assert.InRange(patient.CreatedAt, before, after);
    }

    [Fact]
    public void Constructor_ShouldGenerateUniqueIds()
    {
        var first = CreatePatient();
        var second = CreatePatient();

        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void Constructor_ShouldAllowTodayAsDateOfBirth()
    {
        var today = DateTime.UtcNow.Date;

        var patient = CreatePatient(dateOfBirth: today);

        Assert.Equal(today, patient.DateOfBirth);
    }

    [Fact]
    public void Constructor_ShouldAllowDateOfBirthWithTimeLaterToday()
    {
        var laterToday = DateTime.UtcNow.Date.AddHours(23);

        var patient = CreatePatient(dateOfBirth: laterToday);

        Assert.Equal(laterToday, patient.DateOfBirth);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenDateOfBirthIsTomorrow()
    {
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);

        Assert.Throws<DomainException>(() =>
            CreatePatient(dateOfBirth: tomorrow));
    }

    [Fact]
    public void Constructor_ShouldAllowNullOptionalFields()
    {
        var patient = CreatePatient(
            email: null,
            nationalId: null);

        Assert.Null(patient.Email);
        Assert.Null(patient.NationalId);
    }

    [Fact]
    public void Constructor_ShouldPreserveGender()
    {
        var gender = Enum.GetValues<Gender>()[^1];

        var patient = CreatePatient(gender: gender);

        Assert.Equal(gender, patient.Gender);
    }

    [Fact]
    public void Constructor_CurrentlyAllowsEmptyFirstName()
    {
        // يوثّق السلوك الحالي؛ الـ constructor لا يتحقق من الاسم.
        var patient = CreatePatient(firstName: "");

        Assert.Equal("", patient.FirstName);
    }

    [Fact]
    public void Constructor_CurrentlyAllowsEmptyLastName()
    {
        // يوثّق السلوك الحالي؛ الـ constructor لا يتحقق من الاسم.
        var patient = CreatePatient(lastName: "");

        Assert.Equal("", patient.LastName);
    }

    [Fact]
    public void Constructor_CurrentlyAllowsEmptyPhoneNumber()
    {
        // يوثّق السلوك الحالي؛ الـ constructor لا يتحقق من رقم الهاتف.
        var patient = CreatePatient(phoneNumber: "");

        Assert.Equal("", patient.PhoneNumber);
    }

    [Fact]
    public void UpdateInformation_ShouldUpdateAllEditableFields()
    {
        var patient = CreatePatient();
        var before = DateTime.UtcNow;

        patient.UpdateInformation(
            "Mohamed",
            "Ali",
            "01112345678",
            "mohamed@example.com");

        var after = DateTime.UtcNow;

        Assert.Equal("Mohamed", patient.FirstName);
        Assert.Equal("Ali", patient.LastName);
        Assert.Equal("01112345678", patient.PhoneNumber);
        Assert.Equal("mohamed@example.com", patient.Email);
        
        Assert.NotNull(patient.UpdatedAt);
        Assert.InRange(patient.UpdatedAt.Value, before, after);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void UpdateInformation_ShouldThrow_WhenFirstNameIsInvalid(
        string? firstName)
    {
        var patient = CreatePatient();

        Assert.Throws<DomainException>(() =>
            patient.UpdateInformation(
                firstName!,
                "Ali",
                "01112345678",
                "new@example.com"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void UpdateInformation_ShouldThrow_WhenLastNameIsInvalid(
        string? lastName)
    {
        var patient = CreatePatient();

        Assert.Throws<DomainException>(() =>
            patient.UpdateInformation(
                "Mohamed",
                lastName!,
                "01112345678",
                "new@example.com"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void UpdateInformation_ShouldThrow_WhenPhoneNumberIsInvalid(
        string? phoneNumber)
    {
        var patient = CreatePatient();

        Assert.Throws<DomainException>(() =>
            patient.UpdateInformation(
                "Mohamed",
                "Ali",
                phoneNumber!,
                "new@example.com"));
    }

    [Fact]
    public void UpdateInformation_ShouldAllowClearingEmail()
    {
        var patient = CreatePatient(email: "old@example.com");

        patient.UpdateInformation(
            "Mohamed",
            "Ali",
            "01112345678",
            null);

        Assert.Null(patient.Email);
    }

    [Fact]
    public void UpdateInformation_ShouldNotChangeData_WhenFirstNameIsInvalid()
    {
        var patient = CreatePatient();

        Assert.Throws<DomainException>(() =>
            patient.UpdateInformation(
                "",
                "Changed",
                "01112345678",
                "new@example.com"));

        Assert.Equal("Ahmed", patient.FirstName);
        Assert.Equal("Yasser", patient.LastName);
        Assert.Equal("01012345678", patient.PhoneNumber);
        Assert.Equal("ahmed@example.com", patient.Email);
    }

    [Fact]
    public void UpdateInformation_ShouldNotChangeDateOfBirthOrNationalId()
    {
        var patient = CreatePatient();
        var originalDateOfBirth = patient.DateOfBirth;
        var originalNationalId = patient.NationalId;

        patient.UpdateInformation(
            "Mohamed",
            "Ali",
            "01112345678",
            "new@example.com");

        Assert.Equal(originalDateOfBirth, patient.DateOfBirth);
        Assert.Equal(originalNationalId, patient.NationalId);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        var patient = CreatePatient();

        patient.Deactivate();

        Assert.False(patient.IsActive);
    }

    [Fact]
    public void Deactivate_ShouldUpdateUpdatedAt()
    {
        var patient = CreatePatient();
        var before = DateTime.UtcNow;

        patient.Deactivate();

        var after = DateTime.UtcNow;
        Assert.NotNull(patient.UpdatedAt);
        Assert.InRange(patient.UpdatedAt.Value, before, after);
    }

    [Fact]
    public void Deactivate_ShouldThrow_WhenPatientIsAlreadyInactive()
    {
        var patient = CreatePatient();
        patient.Deactivate();

        Assert.Throws<DomainException>(() =>
            patient.Deactivate());
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        var patient = CreatePatient();
        patient.Deactivate();

        patient.Activate();

        Assert.True(patient.IsActive);
    }

    [Fact]
    public void Activate_ShouldUpdateUpdatedAt()
    {
        var patient = CreatePatient();
        patient.Deactivate();

        var before = DateTime.UtcNow;

        patient.Activate();

        var after = DateTime.UtcNow;
        Assert.NotNull(patient.UpdatedAt);
        Assert.InRange(patient.UpdatedAt.Value, before, after);
    }

    [Fact]
    public void Activate_ShouldThrow_WhenPatientIsAlreadyActive()
    {
        var patient = CreatePatient();

        Assert.Throws<DomainException>(() =>
            patient.Activate());
    }

    [Fact]
    public void ActivateAndDeactivate_ShouldSupportRepeatedLifecycle()
    {
        var patient = CreatePatient();

        patient.Deactivate();
        Assert.False(patient.IsActive);

        patient.Activate();
        Assert.True(patient.IsActive);

        patient.Deactivate();
        Assert.False(patient.IsActive);
    }
}
