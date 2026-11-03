namespace EyeCareAPI.Contracts.MedicalRecords
{

    public sealed record CreateMedicalRecordRequest(
        Guid AppointmentId,
        string Diagnosis,
        string? Notes,
        string? TreatmentPlan,
        DateTime? FollowUpDate);
}
