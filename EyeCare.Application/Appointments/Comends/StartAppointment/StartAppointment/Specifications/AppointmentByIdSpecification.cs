using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EyeCare.Application.Appointments.Comends.StartAppointment.StartAppointment.Specifications
{
    public sealed class AppointmentByIdSpecification
    : ISpecification<Appointment>
    {
        public AppointmentByIdSpecification(Guid appointmentId)
        {
            Criteria = appointment =>
                appointment.Id == appointmentId;
        }

        public Expression<Func<Appointment, bool>> Criteria { get; }
        public Expression<Func<Appointment, object>>?
    OrderByDescending
        { get; } = null;
        public List<Expression<Func<Appointment, object>>> Includes { get; } = [];
    }
}
