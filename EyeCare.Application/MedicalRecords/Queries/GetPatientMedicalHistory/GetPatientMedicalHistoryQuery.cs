using MediatR;

namespace EyeCare.Application.MedicalRecords.Queries.GetPatientMedicalHistory;

public sealed record GetPatientMedicalHistoryQuery(
    Guid PatientId,
    int Page = 1,
    int PageSize = 10)
    : IRequest<PatientMedicalHistoryResponse>;

