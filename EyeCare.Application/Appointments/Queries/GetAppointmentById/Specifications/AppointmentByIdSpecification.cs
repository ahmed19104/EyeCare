using System.Linq.Expressions;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;

namespace EyeCare.Application.Appointments.Queries.GetAppointmentById.Specifications;

public sealed class AppointmentByIdSpecification
    : ISpecification<Appointment>
{
    public AppointmentByIdSpecification(
        Guid appointmentId)
    {
        Criteria = appointment =>
            appointment.Id == appointmentId;

        Includes =
        [
            appointment => appointment.Patient,
            appointment => appointment.Doctor,
            appointment => appointment.Branch
        ];
    }

    public Expression<Func<Appointment, bool>> Criteria { get; }

    public List<Expression<Func<Appointment, object>>> Includes { get; } = new List<Expression<Func<Appointment, object>>>();
    public Expression<Func<Appointment, object>>?
    OrderByDescending
    { get; } = null;
}
