using EyeCare.Application.Abstractions.Authorization;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Appointments.Comends.StartAppointment.StartAppointment.Specifications;
using EyeCare.Application.Exceptions;
using EyeCare.Application.MedicalRecords.Specifications;
using EyeCare.Domain.Entities;
using EyeCare.Domain.Enums;
using MediatR;

namespace EyeCare.Application.MedicalRecords.Commands.CreateMedicalRecord;

public sealed class CreateMedicalRecordCommandHandler
    : IRequestHandler<CreateMedicalRecordCommand, Guid>
{
    private readonly IRepository<Appointment>
        _appointmentRepository;

    private readonly IRepository<MedicalRecord>
        _medicalRecordRepository;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IBranchAuthorizationService
    _branchAuthorizationService;

    public CreateMedicalRecordCommandHandler(
     IRepository<Appointment> appointmentRepository,
     IRepository<MedicalRecord> medicalRecordRepository,
     IUnitOfWork unitOfWork,
     IBranchAuthorizationService branchAuthorizationService)
    {
        _appointmentRepository = appointmentRepository;
        _medicalRecordRepository = medicalRecordRepository;
        _unitOfWork = unitOfWork;
        _branchAuthorizationService =
            branchAuthorizationService;
    }

    public async Task<Guid> Handle(
        CreateMedicalRecordCommand request,
        CancellationToken cancellationToken)
    {
        var appointment =
            await _appointmentRepository.FirstOrDefaultAsync(
                new AppointmentByIdSpecification(
                    request.AppointmentId),
                cancellationToken);

        if (appointment is null)
        {
            throw new NotFoundException(
                "Appointment",
                request.AppointmentId);
        }
       
        
        await _branchAuthorizationService
    .EnsureCanAccessBranchAsync(
        appointment.BranchId,
        cancellationToken);
       
        
        if (appointment.Status != AppointmentStatus.Completed)
        {
            throw new ConflictException(
                "Medical record can only be created for a completed appointment.");
        }

        var existingRecord =
            await _medicalRecordRepository.AnyAsync(
                new MedicalRecordByAppointmentSpecification(
                    request.AppointmentId),
                cancellationToken);

        if (existingRecord)
        {
            throw new ConflictException(
                "A medical record already exists for this appointment.");
        }

        var medicalRecord =
            new MedicalRecord(
                appointment.Id,
                appointment.PatientId,
                appointment.DoctorId,
                request.Diagnosis,
                request.Notes,
                request.TreatmentPlan,
                request.FollowUpDate);

        await _medicalRecordRepository.AddAsync(
            medicalRecord,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return medicalRecord.Id;
    }
}
