namespace EyeCare.Application.MedicalRecords.Queries.GetPatientMedicalHistory;

public sealed record MedicalHistoryItem(
    Guid Id,
    Guid AppointmentId,
    Guid DoctorId,
    string DoctorName,
    string Diagnosis,
    string? TreatmentPlan,
    DateTime? FollowUpDate,
    DateTime CreatedAt);

public sealed record PatientMedicalHistoryResponse(
    IReadOnlyList<MedicalHistoryItem> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
