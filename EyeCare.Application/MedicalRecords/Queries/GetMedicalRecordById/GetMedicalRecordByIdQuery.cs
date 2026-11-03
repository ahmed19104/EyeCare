using MediatR;

namespace EyeCare.Application.MedicalRecords.Queries.GetMedicalRecordById;

public sealed record GetMedicalRecordByIdQuery(
    Guid MedicalRecordId)
    : IRequest<MedicalRecordResponse>;
