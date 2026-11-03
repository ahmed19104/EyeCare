using MediatR;

namespace EyeCare.Application.Appointments.Comends.CheckInAppointment;

public sealed record CheckInAppointmentCommand(
    Guid AppointmentId) : IRequest;
