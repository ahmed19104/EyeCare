
using EyeCare.Domain.Entities;
using EyeCare.Domain.Exceptions;
using Xunit;

namespace EyeCare.Domain.Tests.Entities;

public class MedicalRecordTests
{
    private static MedicalRecord CreateMedicalRecord(
        Guid? appointmentId = null,
        Guid? patientId = null,
        Guid? doctorId = null,
        string diagnosis = "Conjunctivitis",
        string? notes = "Initial examination",
        string? treatmentPlan = "Eye drops",
        DateTime? followUpDate = null)
    {
        return new MedicalRecord(
            appointmentId ?? Guid.NewGuid(),
            patientId ?? Guid.NewGuid(),
            doctorId ?? Guid.NewGuid(),
            diagnosis,
            notes,
            treatmentPlan,
            followUpDate);
    }

    [Fact]
    public void Constructor_ShouldCreateMedicalRecord_WithValidData()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var followUpDate = DateTime.UtcNow.Date.AddDays(7);
        var before = DateTime.UtcNow;

        // Act
        var record = CreateMedicalRecord(
            appointmentId: appointmentId,
            patientId: patientId,
            doctorId: doctorId,
            diagnosis: "Conjunctivitis",
            notes: "Redness and irritation",
            treatmentPlan: "Eye drops twice daily",
            followUpDate: followUpDate);

        var after = DateTime.UtcNow;

        // Assert
        Assert.NotEqual(Guid.Empty, record.Id);
        Assert.Equal(appointmentId, record.AppointmentId);
        Assert.Equal(patientId, record.PatientId);
        Assert.Equal(doctorId, record.DoctorId);
        Assert.Equal("Conjunctivitis", record.Diagnosis);
        Assert.Equal("Redness and irritation", record.Notes);
        Assert.Equal("Eye drops twice daily", record.TreatmentPlan);
        Assert.Equal(followUpDate, record.FollowUpDate);
        Assert.InRange(record.CreatedAt, before, after);
    }

    [Fact]
    public void Constructor_ShouldGenerateUniqueIds()
    {
        var first = CreateMedicalRecord();
        var second = CreateMedicalRecord();

        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenAppointmentIdIsEmpty()
    {
        Assert.Throws<DomainException>(() =>
            CreateMedicalRecord(appointmentId: Guid.Empty));
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenPatientIdIsEmpty()
    {
        Assert.Throws<DomainException>(() =>
            CreateMedicalRecord(patientId: Guid.Empty));
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenDoctorIdIsEmpty()
    {
        Assert.Throws<DomainException>(() =>
            CreateMedicalRecord(doctorId: Guid.Empty));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Constructor_ShouldThrow_WhenDiagnosisIsInvalid(
        string? diagnosis)
    {
        Assert.Throws<DomainException>(() =>
            CreateMedicalRecord(diagnosis: diagnosis!));
    }

    [Fact]
    public void Constructor_ShouldAllowNullOptionalFields()
    {
        var record = CreateMedicalRecord(
            notes: null,
            treatmentPlan: null,
            followUpDate: null);

        Assert.Null(record.Notes);
        Assert.Null(record.TreatmentPlan);
        Assert.Null(record.FollowUpDate);
    }

    [Fact]
    public void Constructor_ShouldAllowTodayAsFollowUpDate()
    {
        var today = DateTime.UtcNow.Date;

        var record = CreateMedicalRecord(followUpDate: today);

        Assert.Equal(today, record.FollowUpDate);
    }

    [Fact]
    public void Constructor_ShouldAllowFutureFollowUpDate()
    {
        var futureDate = DateTime.UtcNow.Date.AddDays(30);

        var record = CreateMedicalRecord(
            followUpDate: futureDate);

        Assert.Equal(futureDate, record.FollowUpDate);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenFollowUpDateIsInPast()
    {
        var pastDate = DateTime.UtcNow.Date.AddDays(-1);

        Assert.Throws<DomainException>(() =>
            CreateMedicalRecord(followUpDate: pastDate));
    }

    [Fact]
    public void Constructor_ShouldCompareFollowUpDateByDateOnly()
    {
        // وقت لاحق اليوم مسموح حتى لو الوقت مختلف عن الوقت الحالي.
        var laterToday = DateTime.UtcNow.Date.AddHours(23);

        var record = CreateMedicalRecord(
            followUpDate: laterToday);

        Assert.Equal(laterToday, record.FollowUpDate);
    }

    [Fact]
    public void Update_ShouldUpdateAllEditableFields()
    {
        // Arrange
        var record = CreateMedicalRecord();
        var followUpDate = DateTime.UtcNow.Date.AddDays(14);
        var before = DateTime.UtcNow;

        // Act
        record.Update(
            "Dry eye syndrome",
            "Symptoms improved",
            "Artificial tears",
            followUpDate);

        var after = DateTime.UtcNow;

        // Assert
        Assert.Equal("Dry eye syndrome", record.Diagnosis);
        Assert.Equal("Symptoms improved", record.Notes);
        Assert.Equal("Artificial tears", record.TreatmentPlan);
        Assert.Equal(followUpDate, record.FollowUpDate);

        Assert.NotNull(record.UpdatedAt);
        Assert.InRange(record.UpdatedAt.Value, before, after);
    }

    [Fact]
    public void Update_ShouldAllowClearingOptionalFields()
    {
        var record = CreateMedicalRecord();

        record.Update(
            "Updated diagnosis",
            null,
            null,
            null);

        Assert.Equal("Updated diagnosis", record.Diagnosis);
        Assert.Null(record.Notes);
        Assert.Null(record.TreatmentPlan);
        Assert.Null(record.FollowUpDate);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Update_ShouldThrow_WhenDiagnosisIsInvalid(
        string? diagnosis)
    {
        var record = CreateMedicalRecord();

        Assert.Throws<DomainException>(() =>
            record.Update(
                diagnosis!,
                "New notes",
                "New treatment",
                null));
    }

    [Fact]
    public void Update_ShouldThrow_WhenFollowUpDateIsInPast()
    {
        var record = CreateMedicalRecord();
        var pastDate = DateTime.UtcNow.Date.AddDays(-1);

        Assert.Throws<DomainException>(() =>
            record.Update(
                "Updated diagnosis",
                "Updated notes",
                "Updated treatment",
                pastDate));
    }

    [Fact]
    public void Update_ShouldNotChangeExistingData_WhenDiagnosisIsInvalid()
    {
        var record = CreateMedicalRecord(
            diagnosis: "Original diagnosis",
            notes: "Original notes",
            treatmentPlan: "Original treatment");

        Assert.Throws<DomainException>(() =>
            record.Update(
                "",
                "Changed notes",
                "Changed treatment",
                null));

        Assert.Equal("Original diagnosis", record.Diagnosis);
        Assert.Equal("Original notes", record.Notes);
        Assert.Equal("Original treatment", record.TreatmentPlan);
    }

    [Fact]
    public void Update_ShouldNotChangeExistingData_WhenFollowUpDateIsInPast()
    {
        var originalFollowUpDate = DateTime.UtcNow.Date.AddDays(10);

        var record = CreateMedicalRecord(
            diagnosis: "Original diagnosis",
            notes: "Original notes",
            treatmentPlan: "Original treatment",
            followUpDate: originalFollowUpDate);

        var pastDate = DateTime.UtcNow.Date.AddDays(-1);

        Assert.Throws<DomainException>(() =>
            record.Update(
                "Changed diagnosis",
                "Changed notes",
                "Changed treatment",
                pastDate));

        Assert.Equal("Original diagnosis", record.Diagnosis);
        Assert.Equal("Original notes", record.Notes);
        Assert.Equal("Original treatment", record.TreatmentPlan);
        Assert.Equal(originalFollowUpDate, record.FollowUpDate);
    }

    [Fact]
    public void Update_ShouldNotChangeAppointmentPatientOrDoctorIds()
    {
        var record = CreateMedicalRecord();

        var originalAppointmentId = record.AppointmentId;
        var originalPatientId = record.PatientId;
        var originalDoctorId = record.DoctorId;

        record.Update(
            "Updated diagnosis",
            "Updated notes",
            "Updated treatment",
            DateTime.UtcNow.Date.AddDays(5));

        Assert.Equal(originalAppointmentId, record.AppointmentId);
        Assert.Equal(originalPatientId, record.PatientId);
        Assert.Equal(originalDoctorId, record.DoctorId);
    }

    [Fact]
    public void Update_ShouldUpdateUpdatedAt()
    {
        var record = CreateMedicalRecord();
        var before = DateTime.UtcNow;

        record.Update(
            "Updated diagnosis",
            null,
            null,
            null);

        var after = DateTime.UtcNow;
        Assert.NotNull(record.UpdatedAt);
        Assert.InRange(record.UpdatedAt.Value, before, after);
    }
}
