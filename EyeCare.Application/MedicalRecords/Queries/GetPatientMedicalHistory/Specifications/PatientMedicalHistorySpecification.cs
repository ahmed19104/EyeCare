using System.Linq.Expressions;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;

namespace EyeCare.Application.MedicalRecords.Queries.GetPatientMedicalHistory.Specifications;

public sealed class PatientMedicalHistorySpecification
    : ISpecification<MedicalRecord>
{
    public PatientMedicalHistorySpecification(
        Guid patientId,
        IReadOnlyCollection<Guid> branchIds)
    {
        Criteria = medicalRecord =>
            medicalRecord.PatientId == patientId &&
            branchIds.Contains(
                medicalRecord.Appointment.BranchId);
    }

    public Expression<Func<MedicalRecord, bool>> Criteria { get; }

    public List<Expression<Func<MedicalRecord, object>>> Includes
        =>
        [
            x => x.Patient,
            x => x.Doctor,
            x => x.Appointment
        ];

    public Expression<Func<MedicalRecord, object>>?
        OrderByDescending
        => x => x.CreatedAt;
}