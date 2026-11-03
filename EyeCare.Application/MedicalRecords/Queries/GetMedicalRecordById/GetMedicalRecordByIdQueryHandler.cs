using EyeCare.Application.Abstractions.Authorization;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Exceptions;
using EyeCare.Application.MedicalRecords.Queries.GetMedicalRecordById.Specifications;
using EyeCare.Application.MedicalRecords.Specifications;
using EyeCare.Domain.Entities;
using MediatR;

namespace EyeCare.Application.MedicalRecords.Queries.GetMedicalRecordById;

public sealed class GetMedicalRecordByIdQueryHandler
    : IRequestHandler<
        GetMedicalRecordByIdQuery,
        MedicalRecordResponse>
{
    private readonly IRepository<MedicalRecord>
        _medicalRecordRepository;
    private readonly IResourceAuthorizationService _resourceAuthorizationService;

    public GetMedicalRecordByIdQueryHandler(
        IRepository<MedicalRecord> medicalRecordRepository, IResourceAuthorizationService resourceAuthorizationService)
    {
        _medicalRecordRepository =
            medicalRecordRepository;
        _resourceAuthorizationService = resourceAuthorizationService;
    }

    public async Task<MedicalRecordResponse> Handle(
        GetMedicalRecordByIdQuery request,
        CancellationToken cancellationToken)
    {
        var medicalRecord =
            await _medicalRecordRepository.FirstOrDefaultAsync(
                new MedicalRecordByIdIcludeSpecification(
                    request.MedicalRecordId),
                cancellationToken);

        if (medicalRecord is null)
        {
            throw new NotFoundException(
                "Medical record",
                request.MedicalRecordId);
        }

        await _resourceAuthorizationService
    .EnsureCanAccessMedicalRecordAsync(
        medicalRecord.Id,
        cancellationToken);

        return new MedicalRecordResponse(
            medicalRecord.Id,
            medicalRecord.AppointmentId,

            medicalRecord.PatientId,
            $"{medicalRecord.Patient.FirstName} {medicalRecord.Patient.LastName}",

            medicalRecord.DoctorId,
            $"{medicalRecord.Doctor.FirstName} {medicalRecord.Doctor.LastName}",

            medicalRecord.Diagnosis,
            medicalRecord.Notes,
            medicalRecord.TreatmentPlan,
            medicalRecord.FollowUpDate,
            medicalRecord.CreatedAt);
    }
}