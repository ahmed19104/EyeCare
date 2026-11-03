using EyeCare.Domain.Common;
using EyeCare.Domain.Exceptions;

namespace EyeCare.Domain.Entities;

public class MedicalRecord : BaseEntity
{
    private MedicalRecord()
    {
    }

    public MedicalRecord(
        Guid appointmentId,
        Guid patientId,
        Guid doctorId,
        string diagnosis,
        string? notes,
        string? treatmentPlan,
        DateTime? followUpDate)
    {
        if (appointmentId == Guid.Empty)
            throw new DomainException(
                "Appointment is required.");

        if (patientId == Guid.Empty)
            throw new DomainException(
                "Patient is required.");

        if (doctorId == Guid.Empty)
            throw new DomainException(
                "Doctor is required.");

        if (string.IsNullOrWhiteSpace(diagnosis))
            throw new DomainException(
                "Diagnosis is required.");

        if (followUpDate.HasValue &&
            followUpDate.Value.Date < DateTime.UtcNow.Date)
        {
            throw new DomainException(
                "Follow-up date cannot be in the past.");
        }

        Id = Guid.NewGuid();

        AppointmentId = appointmentId;
        PatientId = patientId;
        DoctorId = doctorId;
        Diagnosis = diagnosis;
        Notes = notes;
        TreatmentPlan = treatmentPlan;
        FollowUpDate = followUpDate;

        CreatedAt = DateTime.UtcNow;
    }

    public Guid AppointmentId { get; private set; }

    public Guid PatientId { get; private set; }

    public Guid DoctorId { get; private set; }

    public string Diagnosis { get; private set; } = null!;

    public string? Notes { get; private set; }

    public string? TreatmentPlan { get; private set; }

    public DateTime? FollowUpDate { get; private set; }

    public Appointment Appointment { get; private set; } = null!;

    public Patient Patient { get; private set; } = null!;

    public Doctor Doctor { get; private set; } = null!;

    public void Update(
        string diagnosis,
        string? notes,
        string? treatmentPlan,
        DateTime? followUpDate)
    {
        if (string.IsNullOrWhiteSpace(diagnosis))
            throw new DomainException(
                "Diagnosis is required.");

        if (followUpDate.HasValue &&
            followUpDate.Value.Date < DateTime.UtcNow.Date)
        {
            throw new DomainException(
                "Follow-up date cannot be in the past.");
        }

        Diagnosis = diagnosis;
        Notes = notes;
        TreatmentPlan = treatmentPlan;
        FollowUpDate = followUpDate;

        UpdatedAt = DateTime.UtcNow;
    }
}
