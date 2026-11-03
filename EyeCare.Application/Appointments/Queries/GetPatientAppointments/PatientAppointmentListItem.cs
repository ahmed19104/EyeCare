using EyeCare.Domain.Enums;

namespace EyeCare.Application.Appointments.Queries.GetPatientAppointments;

public sealed record PatientAppointmentListItem(
    Guid Id,
    Guid DoctorId,
    string DoctorName,
    Guid BranchId,
    string BranchName,
    DateTime StartAt,
    DateTime EndAt,
    AppointmentStatus Status);
public sealed record PatientAppointmentsResponse(
    IReadOnlyList<PatientAppointmentListItem> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
