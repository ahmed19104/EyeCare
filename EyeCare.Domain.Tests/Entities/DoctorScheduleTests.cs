using EyeCare.Domain.Entities;
using EyeCare.Domain.Enums;
using EyeCare.Domain.Exceptions;

namespace EyeCare.Domain.Tests.Entities;

public class DoctorScheduleTests
{
    [Fact]
    public void Constructor_ShouldCreateSchedule_WhenDataIsValid()
    {
        // Arrange
        var doctorBranchId = Guid.NewGuid();
        var day = ScheduleDay.Sunday;
        var startTime = new TimeSpan(9, 0, 0);
        var endTime = new TimeSpan(13, 0, 0);
        var slotDurationMinutes = 30;

        var beforeCreation = DateTime.UtcNow;

        // Act
        var schedule = new DoctorSchedule(
            doctorBranchId,
            day,
            startTime,
            endTime,
            slotDurationMinutes);

        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.NotEqual(Guid.Empty, schedule.Id);

        Assert.Equal(doctorBranchId, schedule.DoctorBranchId);
        Assert.Equal(day, schedule.Day);
        Assert.Equal(startTime, schedule.StartTime);
        Assert.Equal(endTime, schedule.EndTime);
        Assert.Equal(
            slotDurationMinutes,
            schedule.SlotDurationMinutes);

        Assert.True(schedule.IsActive);

        Assert.InRange(
            schedule.CreatedAt,
            beforeCreation,
            afterCreation);
    }

    [Fact]
    public void Constructor_ShouldGenerateUniqueIds_WhenCreatingDifferentSchedules()
    {
        // Arrange
        var doctorBranchId = Guid.NewGuid();

        // Act
        var firstSchedule = CreateSchedule(doctorBranchId);
        var secondSchedule = CreateSchedule(doctorBranchId);

        // Assert
        Assert.NotEqual(firstSchedule.Id, secondSchedule.Id);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenDoctorBranchIdIsEmpty()
    {
        // Act
        var exception = Assert.Throws<DomainException>(() =>
            new DoctorSchedule(
                Guid.Empty,
                ScheduleDay.Sunday,
                new TimeSpan(9, 0, 0),
                new TimeSpan(13, 0, 0),
                30));

        // Assert
        Assert.Equal(
            "Doctor branch is required.",
            exception.Message);
    }

    [Theory]
    [InlineData(13, 0, 9, 0)]
    [InlineData(9, 0, 9, 0)]
    public void Constructor_ShouldThrow_WhenStartTimeIsNotBeforeEndTime(
        int startHour,
        int startMinute,
        int endHour,
        int endMinute)
    {
        // Act
        var exception = Assert.Throws<DomainException>(() =>
            new DoctorSchedule(
                Guid.NewGuid(),
                ScheduleDay.Sunday,
                new TimeSpan(startHour, startMinute, 0),
                new TimeSpan(endHour, endMinute, 0),
                30));

        // Assert
        Assert.Equal(
            "Schedule start time must be before end time.",
            exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-30)]
    public void Constructor_ShouldThrow_WhenSlotDurationIsNotPositive(
        int slotDurationMinutes)
    {
        // Act
        var exception = Assert.Throws<DomainException>(() =>
            new DoctorSchedule(
                Guid.NewGuid(),
                ScheduleDay.Sunday,
                new TimeSpan(9, 0, 0),
                new TimeSpan(13, 0, 0),
                slotDurationMinutes));

        // Assert
        Assert.Equal(
            "Slot duration must be greater than zero.",
            exception.Message);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenSlotDurationExceedsScheduleDuration()
    {
        // Act
        var exception = Assert.Throws<DomainException>(() =>
            new DoctorSchedule(
                Guid.NewGuid(),
                ScheduleDay.Sunday,
                new TimeSpan(9, 0, 0),
                new TimeSpan(9, 30, 0),
                60));

        // Assert
        Assert.Equal(
            "Slot duration cannot be greater than schedule duration.",
            exception.Message);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenScheduleDurationIsNotDivisibleBySlotDuration()
    {
        // Act
        var exception = Assert.Throws<DomainException>(() =>
            new DoctorSchedule(
                Guid.NewGuid(),
                ScheduleDay.Sunday,
                new TimeSpan(9, 0, 0),
                new TimeSpan(10, 40, 0),
                30));

        // Assert
        Assert.Equal(
            "Schedule duration must be divisible by slot duration.",
            exception.Message);
    }


    [Fact]
    public void Update_ShouldUpdateSchedule_WhenDataIsValid()
    {
        // Arrange
        var schedule = CreateSchedule();

        var newStartTime = new TimeSpan(10, 0, 0);
        var newEndTime = new TimeSpan(14, 0, 0);
        const int newSlotDuration = 60;

        var oldId = schedule.Id;
        var oldDoctorBranchId = schedule.DoctorBranchId;
        var oldDay = schedule.Day;

        var beforeUpdate = DateTime.UtcNow;

        // Act
        schedule.Update(newStartTime, newEndTime, newSlotDuration);

        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.Equal(newStartTime, schedule.StartTime);
        Assert.Equal(newEndTime, schedule.EndTime);
        Assert.Equal(newSlotDuration, schedule.SlotDurationMinutes);

        // Update must not change the schedule identity or its relationship.
        Assert.Equal(oldId, schedule.Id);
        Assert.Equal(oldDoctorBranchId, schedule.DoctorBranchId);
        Assert.Equal(oldDay, schedule.Day);
        Assert.NotNull(schedule.UpdatedAt);
        Assert.InRange(schedule.UpdatedAt.Value, beforeUpdate, afterUpdate);
    }

    [Theory]
    [InlineData(13, 0, 9, 0)]
    [InlineData(9, 0, 9, 0)]
    public void Update_ShouldThrowDomainException_WhenStartTimeIsNotBeforeEndTime(
        int startHour,
        int startMinute,
        int endHour,
        int endMinute)
    {
        // Arrange
        var schedule = CreateSchedule();

        var startTime = new TimeSpan(startHour, startMinute, 0);
        var endTime = new TimeSpan(endHour, endMinute, 0);

        // Act & Assert
        AssertUpdateFailsWithoutChangingData(
            schedule,
            startTime,
            endTime,
            30,
            "Schedule start time must be before end time.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-30)]
    public void Update_ShouldThrowDomainException_WhenSlotDurationIsNotPositive(
        int slotDurationMinutes)
    {
        // Arrange
        var schedule = CreateSchedule();

        // Act & Assert
        AssertUpdateFailsWithoutChangingData(
            schedule,
            new TimeSpan(9, 0, 0),
            new TimeSpan(13, 0, 0),
            slotDurationMinutes,
            "Slot duration must be greater than zero.");
    }

    [Fact]
    public void Update_ShouldThrowDomainException_WhenSlotDurationExceedsScheduleDuration()
    {
        // Arrange
        var schedule = CreateSchedule();

        // Act & Assert
        AssertUpdateFailsWithoutChangingData(
            schedule,
            new TimeSpan(9, 0, 0),
            new TimeSpan(9, 30, 0),
            60,
            "Slot duration cannot be greater than schedule duration.");
    }

    [Fact]
    public void Update_ShouldThrowDomainException_WhenScheduleDurationIsNotDivisibleBySlotDuration()
    {
        // Arrange
        var schedule = CreateSchedule();

        // 100 minutes cannot be divided evenly into 30-minute slots.
        // Act & Assert
        AssertUpdateFailsWithoutChangingData(
            schedule,
            new TimeSpan(9, 0, 0),
            new TimeSpan(10, 40, 0),
            30,
            "Schedule duration must be divisible by slot duration.");
    }

    
[Fact]
public void Activate_ShouldSetIsActiveToTrue_WhenScheduleIsInactive()
    {
        // Arrange
        var schedule = CreateSchedule();
        schedule.Deactivate();

        var beforeActivation = DateTime.UtcNow;

        // Act
        schedule.Activate();

        var afterActivation = DateTime.UtcNow;

        // Assert
        Assert.True(schedule.IsActive);
        Assert.NotNull(schedule.UpdatedAt);
        Assert.InRange(schedule.UpdatedAt.Value, beforeActivation, afterActivation);
    }

    [Fact]
    public void Activate_ShouldNotChangeUpdatedAt_WhenScheduleIsAlreadyActive()
    {
        // Arrange
        var schedule = CreateSchedule();
        var originalUpdatedAt = schedule.UpdatedAt;

        // Act
        schedule.Activate();

        // Assert
        Assert.True(schedule.IsActive);
        Assert.Equal(originalUpdatedAt, schedule.UpdatedAt);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse_WhenScheduleIsActive()
    {
        // Arrange
        var schedule = CreateSchedule();
        var beforeDeactivation = DateTime.UtcNow;

        // Act
        schedule.Deactivate();

        var afterDeactivation = DateTime.UtcNow;

        // Assert
        Assert.False(schedule.IsActive);
        Assert.NotNull(schedule.UpdatedAt);
        Assert.InRange(schedule.UpdatedAt.Value, beforeDeactivation, afterDeactivation);
    }

    [Fact]
    public void Deactivate_ShouldNotChangeUpdatedAt_WhenScheduleIsAlreadyInactive()
    {
        // Arrange
        var schedule = CreateSchedule();
        schedule.Deactivate();

        var originalUpdatedAt = schedule.UpdatedAt;

        // Act
        schedule.Deactivate();

        // Assert
        Assert.False(schedule.IsActive);
        Assert.Equal(originalUpdatedAt, schedule.UpdatedAt);
    }


    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenScheduleDayIsInvalid()
    {
        // Arrange
        var invalidDay = (ScheduleDay)999;

        // Act
        var exception = Assert.Throws<DomainException>(() =>
            new DoctorSchedule(
                Guid.NewGuid(),
                invalidDay,
                new TimeSpan(9, 0, 0),
                new TimeSpan(13, 0, 0),
                30));

        // Assert
        Assert.Equal("Schedule day is invalid.", exception.Message);
    }


    private static void AssertUpdateFailsWithoutChangingData(
        DoctorSchedule schedule,
        TimeSpan startTime,
        TimeSpan endTime,
        int slotDurationMinutes,
        string expectedMessage)
    {
        // Snapshot the current state before attempting the update.
        var oldId = schedule.Id;
        var oldDoctorBranchId = schedule.DoctorBranchId;
        var oldDay = schedule.Day;
        var oldStartTime = schedule.StartTime;
        var oldEndTime = schedule.EndTime;
        var oldSlotDuration = schedule.SlotDurationMinutes;
        var oldIsActive = schedule.IsActive;
        var oldCreatedAt = schedule.CreatedAt;
        var oldUpdatedAt = schedule.UpdatedAt;

        // Act
        var exception = Assert.Throws<DomainException>(() =>
            schedule.Update(startTime, endTime, slotDurationMinutes));

        // Assert: correct business-rule error.
        Assert.Equal(expectedMessage, exception.Message);

        // Assert: a failed operation must not partially mutate the entity.
        Assert.Equal(oldId, schedule.Id);
        Assert.Equal(oldDoctorBranchId, schedule.DoctorBranchId);
        Assert.Equal(oldDay, schedule.Day);
        Assert.Equal(oldStartTime, schedule.StartTime);
        Assert.Equal(oldEndTime, schedule.EndTime);
        Assert.Equal(oldSlotDuration, schedule.SlotDurationMinutes);
        Assert.Equal(oldIsActive, schedule.IsActive);
        Assert.Equal(oldCreatedAt, schedule.CreatedAt);
        Assert.Equal(oldUpdatedAt, schedule.UpdatedAt);
    }




    private static DoctorSchedule CreateSchedule(Guid? doctorBranchId = null)
    {
        return new DoctorSchedule(
            doctorBranchId ?? Guid.NewGuid(),
            ScheduleDay.Sunday,
            new TimeSpan(9, 0, 0),
            new TimeSpan(13, 0, 0),
            30);
    }
}
