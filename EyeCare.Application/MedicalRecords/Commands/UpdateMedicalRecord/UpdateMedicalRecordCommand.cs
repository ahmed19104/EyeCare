using MediatR;

namespace EyeCare.Application.MedicalRecords.Commands.UpdateMedicalRecord;

public sealed record UpdateMedicalRecordCommand(
    Guid MedicalRecordId,
    string Diagnosis,
    string? Notes,
    string? TreatmentPlan,
    DateTime? FollowUpDate)
    : IRequest;
