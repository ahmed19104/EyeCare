using System.Linq.Expressions;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;
using EyeCare.Domain.Enums;

namespace EyeCare.Application.Doctors.Queries.GetAvailableSlots.Specifications;

public sealed class DoctorScheduleByDaySpecification
    : ISpecification<DoctorSchedule>
{
    public DoctorScheduleByDaySpecification(
        Guid doctorBranchId,
        ScheduleDay day)
    {
        Criteria = x =>
            x.DoctorBranchId == doctorBranchId &&
            x.Day == day &&
            x.IsActive;
    }

    public Expression<Func<DoctorSchedule, bool>> Criteria { get; }
    public List<Expression<Func<DoctorSchedule, object>>> Includes { get; } = new List<Expression<Func<DoctorSchedule, object>>>();
    public Expression<Func<DoctorSchedule, object>>?
    OrderByDescending
    { get; } = null;
}
