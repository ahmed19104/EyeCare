using System.Linq.Expressions;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;
using EyeCare.Domain.Enums;

namespace EyeCare.Application.Appointments.Comends.BookAppointment.Specifications;

public sealed class AppointmentAvailabilitySpecification
    : ISpecification<Appointment>
{
    public AppointmentAvailabilitySpecification(
        Guid doctorId,
        DateTime startAt,
        DateTime endAt)
    {
        Criteria = appointment =>
            appointment.DoctorId == doctorId
            &&
            appointment.StartAt < endAt
            &&
            appointment.EndAt > startAt
            &&
            appointment.Status != AppointmentStatus.Cancelled
            &&
            appointment.Status != AppointmentStatus.NoShow;
    }

    public Expression<Func<Appointment, bool>> Criteria { get; }
    

    public List<Expression<Func<Appointment, object>>> Includes { get; } = new List<Expression<Func<Appointment, object>>>();
    public Expression<Func<Appointment, object>>?
    OrderByDescending
    { get; } = null;
}