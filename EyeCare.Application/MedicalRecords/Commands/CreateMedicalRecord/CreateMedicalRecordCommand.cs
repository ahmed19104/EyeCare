using MediatR;

namespace EyeCare.Application.MedicalRecords.Commands.CreateMedicalRecord;

public sealed record CreateMedicalRecordCommand(
    Guid AppointmentId,
    string Diagnosis,
    string? Notes,
    string? TreatmentPlan,
    DateTime? FollowUpDate)
    : IRequest<Guid>;
