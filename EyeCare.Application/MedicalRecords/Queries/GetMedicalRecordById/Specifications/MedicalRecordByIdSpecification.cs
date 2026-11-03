using System.Linq.Expressions;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;

namespace EyeCare.Application.MedicalRecords.Queries.GetMedicalRecordById.Specifications;

public sealed class MedicalRecordByIdIcludeSpecification
    : ISpecification<MedicalRecord>
{
    public MedicalRecordByIdIcludeSpecification(
        Guid medicalRecordId)
    {
        Criteria = record =>
            record.Id == medicalRecordId;

        Includes =
        [
            record => record.Patient,
            record => record.Doctor
            
        ];
    }

    public Expression<Func<MedicalRecord, bool>> Criteria { get; }

    public List<Expression<Func<MedicalRecord, object>>> Includes { get; }
        = [];

    public Expression<Func<MedicalRecord, object>>?
        OrderByDescending
    { get; } = null;
}