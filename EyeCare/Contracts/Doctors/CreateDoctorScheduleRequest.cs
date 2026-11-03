using EyeCare.Domain.Enums;

namespace EyeCareAPI.Contracts.Doctors;

public sealed record CreateDoctorScheduleRequest(
    Guid DoctorId,
    Guid BranchId,
    ScheduleDay Day,
    TimeSpan StartTime,
    TimeSpan EndTime,
    int SlotDurationMinutes);
