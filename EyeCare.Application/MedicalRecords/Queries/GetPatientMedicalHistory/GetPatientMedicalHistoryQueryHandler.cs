using EyeCare.Application.Abstractions.Authorization;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Exceptions;
using EyeCare.Application.MedicalRecords.Queries.GetPatientMedicalHistory.Specifications;
using EyeCare.Application.Patients.Queries.GetPatientById.Specifications;
using EyeCare.Domain.Entities;
using MediatR;

namespace EyeCare.Application.MedicalRecords.Queries.GetPatientMedicalHistory;

public sealed class GetPatientMedicalHistoryQueryHandler
    : IRequestHandler<
        GetPatientMedicalHistoryQuery,
        PatientMedicalHistoryResponse>
{
    private readonly IRepository<MedicalRecord>
        _medicalRecordRepository;

    private readonly IRepository<Patient>
        _patientRepository;
    private readonly IResourceAuthorizationService _resourceAuthorizationService;

    public GetPatientMedicalHistoryQueryHandler(
        IRepository<MedicalRecord> medicalRecordRepository,
        IRepository<Patient> patientRepository, IResourceAuthorizationService resourceAuthorizationService)
    {
        _medicalRecordRepository =
            medicalRecordRepository;

        _patientRepository =
            patientRepository;
        _resourceAuthorizationService = resourceAuthorizationService;
    }

    public async Task<PatientMedicalHistoryResponse> Handle(
        GetPatientMedicalHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var patient =
            await _patientRepository.FirstOrDefaultAsync(
                new PatientByIdSpecification(
                    request.PatientId),
                cancellationToken);

        if (patient is null)
        {
            throw new NotFoundException(
                "Patient",
                request.PatientId);
        }
        var accessibleBranchIds =
    await _resourceAuthorizationService
        .GetAccessibleMedicalRecordBranchIdsAsync(
            cancellationToken);


        var specification =
            new PatientMedicalHistorySpecification(
                request.PatientId, accessibleBranchIds);

        var result =
            await _medicalRecordRepository.ListPagedAsync(
                specification,
                request.Page,
                request.PageSize,
                cancellationToken);

        var items =
            result.Items
                .Select(record =>
                    new MedicalHistoryItem(
                        record.Id,
                        record.AppointmentId,
                        record.DoctorId,
                        $"{record.Doctor.FirstName} {record.Doctor.LastName}",
                        record.Diagnosis,
                        record.TreatmentPlan,
                        record.FollowUpDate,
                        record.CreatedAt))
                .ToList();

        return new PatientMedicalHistoryResponse(
            items,
            result.Page,
            result.PageSize,
            result.TotalCount,
            result.TotalPages);
    }
}
