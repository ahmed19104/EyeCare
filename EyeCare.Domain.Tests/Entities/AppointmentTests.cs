using EyeCare.Domain.Entities;
using EyeCare.Domain.Enums;
using EyeCare.Domain.Exceptions;

namespace EyeCare.Domain.Tests.Entities;

public class AppointmentTests
{
    private static readonly Guid PatientId =
        Guid.NewGuid();

    private static readonly Guid DoctorId =
        Guid.NewGuid();

    private static readonly Guid BranchId =
        Guid.NewGuid();

    private static DateTime FutureStartAt()
    {
        return DateTime.UtcNow.AddHours(1);
    }
    private static Appointment CreateAppointment()
    {
        var startAt =
            DateTime.UtcNow.AddHours(1);

        return new Appointment(
            PatientId,
            DoctorId,
            BranchId,
            startAt,
            startAt.AddMinutes(30));
    }

    [Fact]
    public void Constructor_ShouldCreateScheduledAppointment_WhenDataIsValid()
    {
        // Arrange
        var startAt = FutureStartAt();
        var endAt = startAt.AddMinutes(30);

        // Act
        var appointment = new Appointment(
            PatientId,
            DoctorId,
            BranchId,
            startAt,
            endAt);

        // Assert
        Assert.NotEqual(Guid.Empty, appointment.Id);

        Assert.Equal(
            PatientId,
            appointment.PatientId);

        Assert.Equal(
            DoctorId,
            appointment.DoctorId);

        Assert.Equal(
            BranchId,
            appointment.BranchId);

        Assert.Equal(
            startAt,
            appointment.StartAt);

        Assert.Equal(
            endAt,
            appointment.EndAt);

        Assert.Equal(
            AppointmentStatus.Scheduled,
            appointment.Status);

        Assert.NotEqual(
            default,
            appointment.CreatedAt);
    }
    [Fact]
    public void Constructor_ShouldThrow_WhenPatientIdIsEmpty()
    {
        // Arrange
        var startAt = FutureStartAt();
        var endAt = startAt.AddMinutes(30);

        // Act
        var act = () =>
            new Appointment(
                Guid.Empty,
                DoctorId,
                BranchId,
                startAt,
                endAt);

        // Assert
        var exception =
            Assert.Throws<DomainException>(act);

        Assert.Equal(
            "Patient is required.",
            exception.Message);
    }
    [Fact]
    public void Constructor_ShouldThrow_WhenDoctorIdIsEmpty()
    {
        // Arrange
        var startAt = FutureStartAt();
        var endAt = startAt.AddMinutes(30);

        // Act
        var act = () =>
            new Appointment(
                PatientId,
                Guid.Empty,
                BranchId,
                startAt,
                endAt);

        // Assert
        var exception =
            Assert.Throws<DomainException>(act);

        Assert.Equal(
            "Doctor is required.",
            exception.Message);
    }
    [Fact]
    public void Constructor_ShouldThrow_WhenBranchIdIsEmpty()
    {
        // Arrange
        var startAt = FutureStartAt();
        var endAt = startAt.AddMinutes(30);

        // Act
        var act = () =>
            new Appointment(
                PatientId,
                DoctorId,
                Guid.Empty,
                startAt,
                endAt);

        // Assert
        var exception =
            Assert.Throws<DomainException>(act);

        Assert.Equal(
            "Branch is required.",
            exception.Message);
    }
    [Fact]
    public void Constructor_ShouldThrow_WhenStartAtEqualsEndAt()
    {
        // Arrange
        var startAt = FutureStartAt();

        // Act
        var act = () =>
            new Appointment(
                PatientId,
                DoctorId,
                BranchId,
                startAt,
                startAt);

        // Assert
        var exception =
            Assert.Throws<DomainException>(act);

        Assert.Equal(
            "Appointment start time must be before end time.",
            exception.Message);
    }
    [Fact]
    public void Constructor_ShouldThrow_WhenStartAtIsInThePast()
    {
        // Arrange
        var startAt =
            DateTime.UtcNow.AddMinutes(-10);

        var endAt =
            startAt.AddMinutes(30);

        // Act
        var act = () =>
            new Appointment(
                PatientId,
                DoctorId,
                BranchId,
                startAt,
                endAt);

        // Assert
        var exception =
            Assert.Throws<DomainException>(act);

        Assert.Equal(
            "Appointment cannot be scheduled in the past.",
            exception.Message);
    }
    [Fact]
    public void CheckIn_ShouldChangeStatusToCheckedIn_WhenScheduled()
    {
        // Arrange
        var appointment = CreateAppointment();

        // Act
        appointment.CheckIn();

        // Assert
        Assert.Equal(
            AppointmentStatus.CheckedIn,
            appointment.Status);
    }
    [Fact]
    public void CheckIn_ShouldThrow_WhenAppointmentIsNotScheduled()
    {
        // Arrange
        var appointment = CreateAppointment();

        appointment.CheckIn();

        // Act
        var act = () =>
            appointment.CheckIn();

        // Assert
        var exception =
            Assert.Throws<DomainException>(act);

        Assert.Equal(
            "Only scheduled appointments can be checked in.",
            exception.Message);
    }
    [Fact]
    public void MarkAsNoShow_ShouldChangeStatusToNoShow_WhenAppointmentIsScheduled()
    {
        // Arrange
        var appointment = CreateAppointment();

        // Act
        appointment.MarkAsNoShow();

        // Assert
        Assert.Equal(AppointmentStatus.NoShow, appointment.Status);
    }

    [Fact]
    public void MarkAsNoShow_ShouldThrow_WhenAppointmentIsNotScheduled()
    {
        // Arrange
        var appointment = CreateAppointment();
        appointment.CheckIn();

        // Act & Assert
        Assert.Throws<DomainException>(
            () => appointment.MarkAsNoShow());
    }
    [Fact]
    public void StartConsultation_ShouldChangeStatusToInProgress_WhenAppointmentIsCheckedIn()
    {
        // Arrange
        var appointment = CreateAppointment();
        appointment.CheckIn();

        // Act
        appointment.StartConsultation();

        // Assert
        Assert.Equal(AppointmentStatus.InProgress, appointment.Status);
    }

    [Fact]
    public void StartConsultation_ShouldThrow_WhenAppointmentIsNotCheckedIn()
    {
        // Arrange
        var appointment = CreateAppointment();

        // Act & Assert
        Assert.Throws<DomainException>(
            () => appointment.StartConsultation());
    }
    [Fact]
    public void Complete_ShouldChangeStatusToCompleted_WhenAppointmentIsInProgress()
    {
        // Arrange
        var appointment = CreateAppointment();
        appointment.CheckIn();
        appointment.StartConsultation();

        // Act
        appointment.Complete();

        // Assert
        Assert.Equal(AppointmentStatus.Completed, appointment.Status);
    }

    [Fact]
    public void Complete_ShouldThrow_WhenAppointmentIsNotInProgress()
    {
        // Arrange
        var appointment = CreateAppointment();

        // Act & Assert
        Assert.Throws<DomainException>(
            () => appointment.Complete());
    }
    [Fact]
    public void Cancel_ShouldChangeStatusToCancelled_WhenAppointmentIsScheduled()
    {
        // Arrange
        var appointment = CreateAppointment();

        // Act
        appointment.Cancel();

        // Assert
        Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
    }
    [Fact]
    public void Cancel_ShouldChangeStatusToCancelled_WhenAppointmentIsCheckedIn()
    {
        // Arrange
        var appointment = CreateAppointment();
        appointment.CheckIn();

        // Act
        appointment.Cancel();

        // Assert
        Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
    }
    [Fact]
    public void Cancel_ShouldChangeStatusToCancelled_WhenAppointmentIsInProgress()
    {
        // Arrange
        var appointment = CreateAppointment();
        appointment.CheckIn();
        appointment.StartConsultation();

        // Act
        appointment.Cancel();

        // Assert
        Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
    }
    [Fact]
    public void Cancel_ShouldThrow_WhenAppointmentIsCompleted()
    {
        // Arrange
        var appointment = CreateAppointment();
        appointment.CheckIn();
        appointment.StartConsultation();
        appointment.Complete();

        // Act & Assert
        Assert.Throws<DomainException>(
            () => appointment.Cancel());
    }
    [Fact]
    public void Cancel_ShouldThrow_WhenAppointmentIsNoShow()
    {
        // Arrange
        var appointment = CreateAppointment();
        appointment.MarkAsNoShow();

        // Act & Assert
        Assert.Throws<DomainException>(
            () => appointment.Cancel());
    }
    [Fact]
    public void Cancel_ShouldThrow_WhenAppointmentIsAlreadyCancelled()
    {
        // Arrange
        var appointment = CreateAppointment();
        appointment.Cancel();

        // Act & Assert
        Assert.Throws<DomainException>(
            () => appointment.Cancel());
    }
}