using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeCare.Application.Appointments.Comends.StartAppointment.StartAppointment
{
    public sealed record StartAppointmentCommand(
      Guid AppointmentId) : IRequest;
}
