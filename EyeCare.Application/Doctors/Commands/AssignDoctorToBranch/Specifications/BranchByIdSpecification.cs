using System.Linq.Expressions;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;

namespace EyeCare.Application.Doctors.Commands.AssignDoctorToBranch.Specifications;

public sealed class BranchByIdSpecification
    : ISpecification<Branch>
{
    public BranchByIdSpecification(Guid branchId)
    {
        Criteria = branch => branch.Id == branchId;
    }

    public Expression<Func<Branch, bool>> Criteria { get; }
    public List<Expression<Func<Branch, object>>> Includes { get; } = new List<Expression<Func<Branch, object>>>();
    public Expression<Func<Branch, object>>?
    OrderByDescending
    { get; } = null;
}
