using System.Linq.Expressions;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;

namespace EyeCare.Application.Doctors.Commands.AssignDoctorToBranch.Specifications;

public sealed class DoctorBranchSpecification
    : ISpecification<DoctorBranch>
{
    public DoctorBranchSpecification(
        Guid doctorId,
        Guid branchId)
    {
        Criteria = doctorBranch =>
            doctorBranch.DoctorId == doctorId &&
            doctorBranch.BranchId == branchId;
    }

    public Expression<Func<DoctorBranch, bool>> Criteria { get; }
    public List<Expression<Func<DoctorBranch, object>>> Includes { get; } = new List<Expression<Func<DoctorBranch, object>>>();
    public Expression<Func<DoctorBranch, object>>?
    OrderByDescending
    { get; } = null;
}
