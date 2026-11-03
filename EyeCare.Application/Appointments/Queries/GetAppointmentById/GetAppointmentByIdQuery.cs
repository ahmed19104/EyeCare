using MediatR;

namespace EyeCare.Application.Appointments.Queries.GetAppointmentById;

public sealed record GetAppointmentByIdQuery(
    Guid AppointmentId) : IRequest<AppointmentResponse>;
