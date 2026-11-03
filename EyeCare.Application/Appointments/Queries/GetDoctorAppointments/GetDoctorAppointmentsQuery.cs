using MediatR;

namespace EyeCare.Application.Appointments.Queries.GetDoctorAppointments;

public sealed record GetDoctorAppointmentsQuery(
    Guid DoctorId,
    DateOnly Date,
    int Page = 1,
    int PageSize = 10)
    : IRequest<DoctorAppointmentsResponse>;