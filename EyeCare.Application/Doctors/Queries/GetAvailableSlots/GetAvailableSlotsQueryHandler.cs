using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Doctors.Commands.AssignDoctorToBranch.Specifications;
using EyeCare.Application.Doctors.Queries.GetAvailableSlots.Specifications;
using EyeCare.Application.Exceptions;
using EyeCare.Domain.Entities;
using EyeCare.Domain.Enums;
using MediatR;

namespace EyeCare.Application.Doctors.Queries.GetAvailableSlots;

public sealed class GetAvailableSlotsQueryHandler
    : IRequestHandler<
        GetAvailableSlotsQuery,
        IReadOnlyList<TimeSpan>>
{
    private readonly IRepository<Doctor> _doctorRepository;
    private readonly IRepository<DoctorBranch> _doctorBranchRepository;
    private readonly IRepository<DoctorSchedule> _scheduleRepository;
    private readonly IRepository<Appointment> _appointmentRepository;

    public GetAvailableSlotsQueryHandler(
        IRepository<Doctor> doctorRepository,
        IRepository<DoctorBranch> doctorBranchRepository,
        IRepository<DoctorSchedule> scheduleRepository,
        IRepository<Appointment> appointmentRepository)
    {
        _doctorRepository = doctorRepository;
        _doctorBranchRepository = doctorBranchRepository;
        _scheduleRepository = scheduleRepository;
        _appointmentRepository = appointmentRepository;
    }

    public async Task<IReadOnlyList<TimeSpan>> Handle(
        GetAvailableSlotsQuery request,
        CancellationToken cancellationToken)
    {
        var doctor = await _doctorRepository.FirstOrDefaultAsync(
            new DoctorByIdSpecification(request.DoctorId),
            cancellationToken);

        if (doctor is null)
        {
            throw new NotFoundException(
                "Doctor",
                request.DoctorId);
        }

        if (!doctor.IsActive)
        {
            throw new ConflictException(
                "Doctor is not active.");
        }

        var doctorBranch =
            await _doctorBranchRepository.FirstOrDefaultAsync(
                new ActiveDoctorBranchSpecification(
                    request.DoctorId,
                    request.BranchId),
                cancellationToken);

        if (doctorBranch is null)
        {
            throw new ConflictException(
                "Doctor is not actively assigned to this branch.");
        }

        var day = (ScheduleDay)request.Date.DayOfWeek;

        var schedule =
            await _scheduleRepository.FirstOrDefaultAsync(
                new DoctorScheduleByDaySpecification(
                    doctorBranch.Id,
                    day),
                cancellationToken);

        if (schedule is null)
        {
            return [];
        }

        var dayStart = request.Date
            .ToDateTime(TimeOnly.MinValue);

        var dayEnd = request.Date
            .AddDays(1)
            .ToDateTime(TimeOnly.MinValue);

        var appointments =
            await _appointmentRepository.ListAsync(
                new AppointmentByDoctorAndDateSpecification(
                    request.DoctorId,
                    dayStart,
                    dayEnd),
                cancellationToken);

        var availableSlots = new List<TimeSpan>();

        var slot = schedule.StartTime;

        while (slot < schedule.EndTime)
        {
            var slotEnd =
                slot.Add(
                    TimeSpan.FromMinutes(
                        schedule.SlotDurationMinutes));

            var hasConflict = appointments.Any(
                appointment =>
                    appointment.StartAt.TimeOfDay < slotEnd &&
                    appointment.EndAt.TimeOfDay > slot);

            if (!hasConflict)
            {
                availableSlots.Add(slot);
            }

            slot = slotEnd;
        }

        return availableSlots;
    }
}
