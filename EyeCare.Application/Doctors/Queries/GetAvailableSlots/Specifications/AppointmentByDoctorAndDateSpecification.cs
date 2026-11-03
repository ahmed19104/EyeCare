using System.Linq.Expressions;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;
using EyeCare.Domain.Enums;

namespace EyeCare.Application.Doctors.Queries.GetAvailableSlots.Specifications;

public sealed class AppointmentByDoctorAndDateSpecification
    : ISpecification<Appointment>
{
    public AppointmentByDoctorAndDateSpecification(
        Guid doctorId,
        DateTime dayStart,
        DateTime dayEnd)
    {
        Criteria = appointment =>
            appointment.DoctorId == doctorId &&
            appointment.StartAt < dayEnd &&
            appointment.EndAt > dayStart &&
            appointment.Status != AppointmentStatus.Cancelled &&
            appointment.Status != AppointmentStatus.NoShow;
    }

    public Expression<Func<Appointment, bool>> Criteria { get; }
    public List<Expression<Func<Appointment, object>>> Includes { get; } = new List<Expression<Func<Appointment, object>>>();
    public Expression<Func<Appointment, object>>?
    OrderByDescending
    { get; } = null;
}