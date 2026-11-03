using System.Linq.Expressions;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;
using EyeCare.Domain.Enums;

namespace EyeCare.Application.Appointments.Comends.BookAppointment.Specifications;

public sealed class AppointmentByDoctorAndSlotSpecification
    : ISpecification<Appointment>
{
    public AppointmentByDoctorAndSlotSpecification(
        Guid doctorId,
        DateTime startAt)
    {
        Criteria = appointment =>
            appointment.DoctorId == doctorId &&
            appointment.StartAt == startAt &&
            appointment.Status != AppointmentStatus.Cancelled &&
            appointment.Status != AppointmentStatus.NoShow;
    }

    public Expression<Func<Appointment, bool>> Criteria { get; }
    public List<Expression<Func<Appointment, object>>> Includes { get; } = new List<Expression<Func<Appointment, object>>>();
    public Expression<Func<Appointment, object>>?
    OrderByDescending
    { get; } = null;
}
