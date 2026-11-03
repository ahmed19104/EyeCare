namespace EyeCareAPI.Contracts.MedicalRecords
{
    public sealed record UpdateMedicalRecordRequest(
    string Diagnosis,
    string? Notes,
    string? TreatmentPlan,
    DateTime? FollowUpDate);
}
