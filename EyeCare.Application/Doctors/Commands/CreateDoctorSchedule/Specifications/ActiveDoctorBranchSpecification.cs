using System.Linq.Expressions;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;

namespace EyeCare.Application.Doctors.Commands.CreateDoctorSchedule.Specifications;

public sealed class ActiveDoctorBranchSpecification
    : ISpecification<DoctorBranch>
{
    public ActiveDoctorBranchSpecification(
        Guid doctorId,
        Guid branchId)
    {
        Criteria = x =>
            x.DoctorId == doctorId &&
            x.BranchId == branchId &&
            x.IsActive;
    }

    public Expression<Func<DoctorBranch, bool>> Criteria { get; }
    public List<Expression<Func<DoctorBranch, object>>> Includes { get; } = new List<Expression<Func<DoctorBranch, object>>>();
    public Expression<Func<DoctorBranch, object>>?
    OrderByDescending
    { get; } = null;
}
