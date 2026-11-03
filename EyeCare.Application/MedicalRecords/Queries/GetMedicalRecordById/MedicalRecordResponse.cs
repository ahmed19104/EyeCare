namespace EyeCare.Application.MedicalRecords.Queries.GetMedicalRecordById;

public sealed record MedicalRecordResponse(
    Guid Id,
    Guid AppointmentId,
    Guid PatientId,
    string PatientName,
    Guid DoctorId,
    string DoctorName,
    string Diagnosis,
    string? Notes,
    string? TreatmentPlan,
    DateTime? FollowUpDate,
    DateTime CreatedAt);
