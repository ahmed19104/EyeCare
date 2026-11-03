using System.Linq.Expressions;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;

namespace EyeCare.Application.Appointments.Comends.BookAppointment.Specifications;

public sealed class ActiveDoctorBranchSpecification
    : ISpecification<DoctorBranch>
{
    public ActiveDoctorBranchSpecification(
        Guid doctorId,
        Guid branchId)
    {
        Criteria = doctorBranch =>
            doctorBranch.DoctorId == doctorId
            && doctorBranch.BranchId == branchId
            && doctorBranch.IsActive;
    }

    public Expression<Func<DoctorBranch, bool>> Criteria { get; }

    public List<Expression<Func<DoctorBranch, object>>> Includes { get; } = new List<Expression<Func<DoctorBranch, object>>>();
    public Expression<Func<DoctorBranch, object>>?
    OrderByDescending
    { get; } = null;
}
