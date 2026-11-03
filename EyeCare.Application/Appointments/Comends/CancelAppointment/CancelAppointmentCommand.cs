using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeCare.Application.Appointments.Comends.CancelAppointment
{
    public sealed record CancelAppointmentCommand(
        Guid AppointmentId) : IRequest;
}
