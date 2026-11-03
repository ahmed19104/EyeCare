using MediatR;

namespace EyeCare.Application.Appointments.Comends.CompleteAppointment
{
    

    

    public sealed record CompleteAppointmentCommand(
        Guid AppointmentId) : IRequest;
}
