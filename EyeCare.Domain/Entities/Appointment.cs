using EyeCare.Domain.Common;
using EyeCare.Domain.Enums;
using EyeCare.Domain.Exceptions;


namespace EyeCare.Domain.Entities
{
    public class Appointment : BaseEntity
    {
        private Appointment()
        {
        }

        public Appointment(
            Guid patientId,
            Guid doctorId,
            Guid branchId,
            DateTime startAt,
            DateTime endAt)
        {
            if (patientId == Guid.Empty)
                throw new DomainException("Patient is required.");

            if (doctorId == Guid.Empty)
                throw new DomainException("Doctor is required.");

            if (branchId == Guid.Empty)
                throw new DomainException("Branch is required.");

            if (startAt >= endAt)
                throw new DomainException(
                    "Appointment start time must be before end time.");

            if (startAt < DateTime.UtcNow)
                throw new DomainException(
                    "Appointment cannot be scheduled in the past.");

            Id = Guid.NewGuid();

            PatientId = patientId;
            DoctorId = doctorId;
            BranchId = branchId;

            StartAt = startAt;
            EndAt = endAt;

            Status = AppointmentStatus.Scheduled;

            CreatedAt = DateTime.UtcNow;
        }

        public Guid PatientId { get; private set; }

        public Guid DoctorId { get; private set; }

        public Guid BranchId { get; private set; }

        public DateTime StartAt { get; private set; }
        public DateTime SlotStartAt => StartAt;
        public DateTime EndAt { get; private set; }

        public AppointmentStatus Status { get; private set; }

        public Patient Patient { get; private set; } = null!;

        public Doctor Doctor { get; private set; } = null!;

        public Branch Branch { get; private set; } = null!;

        public void CheckIn()
        {
            if (Status != AppointmentStatus.Scheduled)
            {
                throw new DomainException(
                    "Only scheduled appointments can be checked in.");
            }

            Status = AppointmentStatus.CheckedIn;
            UpdatedAt = DateTime.UtcNow;
        }
        public void MarkAsNoShow() 
        {
            if (Status != AppointmentStatus.Scheduled) 
            {
                throw new DomainException(
                    "Only scheduled appointments can be MarkAs NoShow.");
            }
            Status = AppointmentStatus.NoShow;
            UpdatedAt = DateTime.UtcNow;
        }
        public void StartConsultation()
        {
            if (Status != AppointmentStatus.CheckedIn)
            {
                throw new DomainException(
                    "Appointment must be checked in before consultation starts.");
            }

            Status = AppointmentStatus.InProgress;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Complete()
        {
            if (Status != AppointmentStatus.InProgress)
            {
                throw new DomainException(
                    "Only appointments in progress can be completed.");
            }

            Status = AppointmentStatus.Completed;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Cancel()
        {
            if (Status is AppointmentStatus.Completed
                or AppointmentStatus.Cancelled
                or AppointmentStatus.NoShow)
            {
                throw new DomainException(
                    "This appointment cannot be cancelled.");
            }

            Status = AppointmentStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
