namespace EyeCareAPI.Contracts.Appointments
{


    public sealed record BookAppointmentRequest(
        Guid PatientId,
        Guid DoctorId,
        Guid BranchId,
        DateOnly Date,
        TimeSpan StartTime);
}
