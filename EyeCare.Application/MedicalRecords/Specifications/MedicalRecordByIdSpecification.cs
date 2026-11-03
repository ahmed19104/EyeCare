using System.Linq.Expressions;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;

namespace EyeCare.Application.MedicalRecords.Specifications;

public sealed class MedicalRecordByIdSpecification
    : ISpecification<MedicalRecord>
{
    public MedicalRecordByIdSpecification(
        Guid medicalRecordId)
    {
        Criteria = record =>
            record.Id == medicalRecordId;
    }

    public Expression<Func<MedicalRecord, bool>> Criteria { get; }

    public List<Expression<Func<MedicalRecord, object>>> Includes { get; }
        = [];

    public Expression<Func<MedicalRecord, object>>?
        OrderByDescending
    { get; } = null;
}
