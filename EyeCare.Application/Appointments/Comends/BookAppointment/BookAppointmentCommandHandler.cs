using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Appointments.Comends.BookAppointment.Specifications;
//using EyeCare.Application.Doctors.Queries.GetAvailableSlots.Specifications;
using EyeCare.Application.Exceptions;
using EyeCare.Domain.Entities;
using EyeCare.Domain.Enums;
using MediatR;

namespace EyeCare.Application.Appointments.Comends.BookAppointment;

public sealed class BookAppointmentCommandHandler
    : IRequestHandler<BookAppointmentCommand, Guid>
{
    private readonly IRepository<Patient> _patientRepository;
    private readonly IRepository<Doctor> _doctorRepository;
    private readonly IRepository<Branch> _branchRepository;
    private readonly IRepository<DoctorBranch> _doctorBranchRepository;
    private readonly IRepository<DoctorSchedule> _scheduleRepository;
    private readonly IRepository<Appointment> _appointmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BookAppointmentCommandHandler(
        IRepository<Patient> patientRepository,
        IRepository<Doctor> doctorRepository,
        IRepository<Branch> branchRepository,
        IRepository<DoctorBranch> doctorBranchRepository,
        IRepository<DoctorSchedule> scheduleRepository,
        IRepository<Appointment> appointmentRepository,
        IUnitOfWork unitOfWork)
    {
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
        _branchRepository = branchRepository;
        _doctorBranchRepository = doctorBranchRepository;
        _scheduleRepository = scheduleRepository;
        _appointmentRepository = appointmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        BookAppointmentCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Validate patient
        var patient =
            await _patientRepository.FirstOrDefaultAsync(
                new PatientByIdSpecification(request.PatientId),
                cancellationToken);

        if (patient is null)
        {
            throw new NotFoundException(
                "Patient",
                request.PatientId);
        }

        // 2. Validate doctor
        var doctor =
            await _doctorRepository.FirstOrDefaultAsync(
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

        // 3. Validate branch
        var branch =
            await _branchRepository.FirstOrDefaultAsync(
                new BranchByIdSpecification(request.BranchId),
                cancellationToken);

        if (branch is null)
        {
            throw new NotFoundException(
                "Branch",
                request.BranchId);
        }

        if (!branch.IsActive)
        {
            throw new ConflictException(
                "Branch is not active.");
        }

        // 4. Validate doctor belongs to branch
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

        // 5. Get doctor's schedule for this day
        var scheduleDay =
            (ScheduleDay)request.Date.DayOfWeek;

        var schedule =
            await _scheduleRepository.FirstOrDefaultAsync(
                new Doctors.Queries.GetAvailableSlots.Specifications.DoctorScheduleByDaySpecification(
                    doctorBranch.Id,
                    scheduleDay),
                cancellationToken);

        if (schedule is null)
        {
            throw new ConflictException(
                "Doctor does not have a schedule on this day.");
        }

        // 6. Validate requested date is not in the past
        var now = DateTime.Now;
        var today = DateOnly.FromDateTime(now);

        if (request.Date < today)
        {
            throw new ConflictException("Appointment date cannot be in the past.");
        }

        // التعديل هنا: لو الحجز لنفس اليوم، نتأكد إن الوقت لسه مجاش
        if (request.Date == today && request.StartTime <= now.TimeOfDay)
        {
            throw new ConflictException("Appointment time has already passed.");
        }

        // 7. Validate slot is inside schedule
        if (request.StartTime < schedule.StartTime)
        {
            throw new ConflictException(
                "Appointment slot is outside the doctor's schedule.");
        }

        // 8. Calculate slot end time
        var slotEndTime =
            request.StartTime.Add(
                TimeSpan.FromMinutes(
                    schedule.SlotDurationMinutes));

        if (slotEndTime > schedule.EndTime)
        {
            throw new ConflictException(
                "Appointment slot exceeds the doctor's schedule.");
        }

        // 9. Validate slot alignment
        var minutesFromScheduleStart =
            (request.StartTime - schedule.StartTime)
            .TotalMinutes;

        if (minutesFromScheduleStart %
            schedule.SlotDurationMinutes != 0)
        {
            throw new ConflictException(
                "Appointment time does not match a valid slot.");
        }

        // 10. Build actual DateTime values
        var startAt =
            request.Date.ToDateTime(
                TimeOnly.FromTimeSpan(
                    request.StartTime));

        var endAt =
            request.Date.ToDateTime(
                TimeOnly.FromTimeSpan(
                    slotEndTime));

        // 11. Check application-level availability
        var existingAppointment =
            await _appointmentRepository.FirstOrDefaultAsync(
                new AppointmentByDoctorAndSlotSpecification(
                    request.DoctorId,
                    startAt),
                cancellationToken);

        if (existingAppointment is not null)
        {
            throw new ConflictException(
                "Doctor already has an appointment for this time slot.");
        }

        // 12. Create appointment
        var appointment = new Appointment(
            request.PatientId,
            request.DoctorId,
            request.BranchId,
            startAt,
            endAt);

        // 13. Add
        await _appointmentRepository.AddAsync(
            appointment,
            cancellationToken);

        // 14. Save
        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return appointment.Id;
    }
}