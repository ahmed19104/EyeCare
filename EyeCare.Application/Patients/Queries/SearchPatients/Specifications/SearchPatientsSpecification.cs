using System.Linq.Expressions;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;

namespace EyeCare.Application.Patients.Queries.SearchPatients.Specifications;

public sealed class SearchPatientsSpecification
    : ISpecification<Patient>
{
    public SearchPatientsSpecification(
        string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            Criteria = patient =>
                patient.IsActive;

            return;
        }

        search = search.Trim();

        Criteria = patient =>
            patient.IsActive &&
            (
                patient.FirstName.Contains(search) ||
                patient.LastName.Contains(search) ||
                patient.PhoneNumber.Contains(search) ||
                (patient.NationalId != null &&
                 patient.NationalId.Contains(search))
            );
    }

    public Expression<Func<Patient, bool>> Criteria { get; }
    public List<Expression<Func<Patient, object>>> Includes { get; } = new List<Expression<Func<Patient, object>>>();
    public Expression<Func<Patient, object>>?
    OrderByDescending
    { get; } = null;
}