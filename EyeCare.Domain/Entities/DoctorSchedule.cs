using EyeCare.Domain.Common;
using EyeCare.Domain.Enums;
using EyeCare.Domain.Exceptions;

namespace EyeCare.Domain.Entities;

public class DoctorSchedule : BaseEntity
{
    private DoctorSchedule()
    {
    }

    public DoctorSchedule(
        Guid doctorBranchId,
        ScheduleDay day,
        TimeSpan startTime,
        TimeSpan endTime,
        int slotDurationMinutes)
    {
        if (doctorBranchId == Guid.Empty)
        {
            throw new DomainException(
                "Doctor branch is required.");
        }
        if (!Enum.IsDefined(day))
        {
            throw new DomainException("Schedule day is invalid.");
        }

        if (startTime >= endTime)
        {
            throw new DomainException(
                "Schedule start time must be before end time.");
        }

        if (slotDurationMinutes <= 0)
        {
            throw new DomainException(
                "Slot duration must be greater than zero.");
        }

        var totalMinutes =
            (endTime - startTime).TotalMinutes;

        if (slotDurationMinutes > totalMinutes)
        {
            throw new DomainException(
                "Slot duration cannot be greater than schedule duration.");
        }

        if (totalMinutes % slotDurationMinutes != 0)
        {
            throw new DomainException(
                "Schedule duration must be divisible by slot duration.");
        }

        Id = Guid.NewGuid();

        DoctorBranchId = doctorBranchId;
        Day = day;
        StartTime = startTime;
        EndTime = endTime;
        SlotDurationMinutes = slotDurationMinutes;

        IsActive = true;

        CreatedAt = DateTime.UtcNow;
    }

    public Guid DoctorBranchId { get; private set; }

    public ScheduleDay Day { get; private set; }

    public TimeSpan StartTime { get; private set; }

    public TimeSpan EndTime { get; private set; }

    public int SlotDurationMinutes { get; private set; }

    public bool IsActive { get; private set; }

    public DoctorBranch DoctorBranch { get; private set; } = null!;

    public void Update(
        TimeSpan startTime,
        TimeSpan endTime,
        int slotDurationMinutes)
    {
        if (startTime >= endTime)
        {
            throw new DomainException(
                "Schedule start time must be before end time.");
        }

        if (slotDurationMinutes <= 0)
        {
            throw new DomainException(
                "Slot duration must be greater than zero.");
        }

        var totalMinutes =
            (endTime - startTime).TotalMinutes;

        if (slotDurationMinutes > totalMinutes)
        {
            throw new DomainException(
                "Slot duration cannot be greater than schedule duration.");
        }

        if (totalMinutes % slotDurationMinutes != 0)
        {
            throw new DomainException(
                "Schedule duration must be divisible by slot duration.");
        }

        StartTime = startTime;
        EndTime = endTime;
        SlotDurationMinutes = slotDurationMinutes;

        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsActive)
            return;
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
            return;
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
