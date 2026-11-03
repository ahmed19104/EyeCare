using MediatR;

namespace EyeCare.Application.Appointments.Comends.MarkAppointmentAsNoShow
{
    public sealed record MarkAppointmentAsNoShowCommand(
        Guid AppointmentId) : IRequest;
}
