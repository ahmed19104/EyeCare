using System.Linq.Expressions;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;

namespace EyeCare.Application.MedicalRecords.Specifications;

public sealed class MedicalRecordByAppointmentSpecification
    : ISpecification<MedicalRecord>
{
    public MedicalRecordByAppointmentSpecification(
        Guid appointmentId)
    {
        Criteria = record =>
            record.AppointmentId == appointmentId;
    }

    public Expression<Func<MedicalRecord, bool>> Criteria { get; }

    public List<Expression<Func<MedicalRecord, object>>> Includes { get; }
        = [];

    public Expression<Func<MedicalRecord, object>>?
        OrderByDescending
    { get; } = null;
}
