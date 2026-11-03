using System.Linq.Expressions;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;

namespace EyeCare.Application.Appointments.Queries.GetDoctorAppointments.Specifications;

public sealed class DoctorAppointmentsByDateSpecification
    : ISpecification<Appointment>
{
    public DoctorAppointmentsByDateSpecification(
        Guid doctorId,
        DateOnly date)
    {
        var startOfDay = date.ToDateTime(TimeOnly.MinValue);
        var endOfDay = startOfDay.AddDays(1);

        Criteria = appointment =>
            appointment.DoctorId == doctorId &&
            appointment.StartAt >= startOfDay &&
            appointment.StartAt < endOfDay;

        Includes =
        [
            appointment => appointment.Patient
        ];
    }

    public Expression<Func<Appointment, bool>> Criteria { get; }

    public List<Expression<Func<Appointment, object>>> Includes { get; } = new List<Expression<Func<Appointment, object>>>();
    public Expression<Func<Appointment, object>>?
    OrderByDescending
    { get; } = null;
}
