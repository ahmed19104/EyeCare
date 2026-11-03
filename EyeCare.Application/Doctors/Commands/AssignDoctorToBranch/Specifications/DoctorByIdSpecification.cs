using System.Linq.Expressions;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;

namespace EyeCare.Application.Doctors.Commands.AssignDoctorToBranch.Specifications;

public sealed class DoctorByIdSpecification
    : ISpecification<Doctor>
{
    public DoctorByIdSpecification(Guid doctorId)
    {
        Criteria = doctor => doctor.Id == doctorId;
    }

    public Expression<Func<Doctor, bool>> Criteria { get; }
    public List<Expression<Func<Doctor, object>>> Includes { get; } = new List<Expression<Func<Doctor, object>>>();
    public Expression<Func<Doctor, object>>?
    OrderByDescending
    { get; } = null;
}
