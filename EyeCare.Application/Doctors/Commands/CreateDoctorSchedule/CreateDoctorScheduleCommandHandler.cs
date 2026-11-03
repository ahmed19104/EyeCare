using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Doctors.Commands.CreateDoctorSchedule.Specifications;
using EyeCare.Application.Exceptions;
using EyeCare.Domain.Entities;
using MediatR;

namespace EyeCare.Application.Doctors.Commands.CreateDoctorSchedule;

public sealed class CreateDoctorScheduleCommandHandler
    : IRequestHandler<CreateDoctorScheduleCommand, Guid>
{
    private readonly IRepository<DoctorBranch> _doctorBranchRepository;
    private readonly IRepository<DoctorSchedule> _scheduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDoctorScheduleCommandHandler(
        IRepository<DoctorBranch> doctorBranchRepository,
        IRepository<DoctorSchedule> scheduleRepository,
        IUnitOfWork unitOfWork)
    {
        _doctorBranchRepository = doctorBranchRepository;
        _scheduleRepository = scheduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateDoctorScheduleCommand request,
        CancellationToken cancellationToken)
    {
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

        var existingSchedule =
            await _scheduleRepository.FirstOrDefaultAsync(
                new DoctorScheduleByDaySpecification(
                    doctorBranch.Id,
                    request.Day),
                cancellationToken);

        if (existingSchedule is not null)
        {
            throw new ConflictException(
                "A schedule already exists for this doctor on this day.");
        }

        var schedule = new DoctorSchedule(
            doctorBranch.Id,
            request.Day,
            request.StartTime,
            request.EndTime,
            request.SlotDurationMinutes);

        await _scheduleRepository.AddAsync(
            schedule,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return schedule.Id;
    }
}
