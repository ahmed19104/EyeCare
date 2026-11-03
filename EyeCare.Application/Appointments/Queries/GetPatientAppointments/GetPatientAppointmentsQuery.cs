using MediatR;

namespace EyeCare.Application.Appointments.Queries.GetPatientAppointments;

public sealed record GetPatientAppointmentsQuery(
    Guid PatientId,
    int Page = 1,
    int PageSize = 10)
    : IRequest<PatientAppointmentsResponse>;
