using MediatR;

namespace EyeCare.Application.MedicalRecords.Queries
    .GetMedicalRecordAuditHistory;

public sealed record GetMedicalRecordAuditHistoryQuery(
    Guid MedicalRecordId)
    : IRequest<IReadOnlyList<AuditHistoryItem>>;
