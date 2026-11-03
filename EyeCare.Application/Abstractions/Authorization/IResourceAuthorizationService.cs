namespace EyeCare.Application.Abstractions.Authorization;

public interface IResourceAuthorizationService
{
    Task EnsureCanAccessAppointmentAsync(
        Guid appointmentId,
        CancellationToken cancellationToken = default);

    Task EnsureCanAccessMedicalRecordAsync(
        Guid medicalRecordId,
        CancellationToken cancellationToken = default);

    Task EnsureCanAccessPatientAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Guid>> GetAccessibleMedicalRecordBranchIdsAsync(
       CancellationToken cancellationToken = default);

}