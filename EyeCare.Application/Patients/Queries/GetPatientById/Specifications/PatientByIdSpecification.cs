using System.Linq.Expressions;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;

namespace EyeCare.Application.Patients.Queries.GetPatientById.Specifications;

public sealed class PatientByIdSpecification
    : ISpecification<Patient>
{
    public PatientByIdSpecification(Guid patientId)
    {
        Criteria = patient =>
            patient.Id == patientId;
    }

    public Expression<Func<Patient, bool>> Criteria { get; }
    public List<Expression<Func<Patient, object>>> Includes { get; } = new List<Expression<Func<Patient, object>>>();
    public Expression<Func<Patient, object>>?
    OrderByDescending
    { get; } = null;
}
