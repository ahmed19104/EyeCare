using System.Linq.Expressions;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;

namespace EyeCare.Application.Appointments.Queries.GetPatientAppointments.Specifications;

public sealed class PatientAppointmentsSpecification
    : ISpecification<Appointment>
{
    public PatientAppointmentsSpecification(
        Guid patientId)
    {
        Criteria = appointment =>
            appointment.PatientId == patientId;

        Includes =
        [
            appointment => appointment.Doctor,
            appointment => appointment.Branch
        ];

        OrderByDescending =
            appointment => appointment.StartAt;
    }

    public Expression<Func<Appointment, bool>> Criteria { get; }

    public List<Expression<Func<Appointment, object>>> Includes { get; }
        = [];

    public Expression<Func<Appointment, object>>
        OrderByDescending
    { get; }
}