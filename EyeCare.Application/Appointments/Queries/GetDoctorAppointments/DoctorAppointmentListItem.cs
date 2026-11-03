using EyeCare.Domain.Enums;

namespace EyeCare.Application.Appointments.Queries.GetDoctorAppointments;

public sealed record DoctorAppointmentListItem(
    Guid Id,
    Guid PatientId,
    string PatientName,
    DateTime StartAt,
    DateTime EndAt,
    AppointmentStatus Status);
public sealed record DoctorAppointmentsResponse(
    IReadOnlyList<DoctorAppointmentListItem> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
