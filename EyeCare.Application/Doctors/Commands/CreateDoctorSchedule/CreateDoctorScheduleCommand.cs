using EyeCare.Domain.Enums;
using MediatR;

namespace EyeCare.Application.Doctors.Commands.CreateDoctorSchedule;

public sealed record CreateDoctorScheduleCommand(
    Guid DoctorId,
    Guid BranchId,
    ScheduleDay Day,
    TimeSpan StartTime,
    TimeSpan EndTime,
    int SlotDurationMinutes) : IRequest<Guid>;