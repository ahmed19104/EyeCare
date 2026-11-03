using EyeCare.Domain.Enums;

namespace EyeCare.Application.Appointments.Queries.GetAppointmentById;

public sealed record AppointmentResponse(
    Guid Id,
    Guid PatientId,
    string PatientName,
    Guid DoctorId,
    string DoctorName,
    Guid BranchId,
    string BranchName,
    DateTime StartAt,
    DateTime EndAt,
    AppointmentStatus Status);