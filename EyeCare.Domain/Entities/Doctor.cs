using EyeCare.Domain.Common;
using EyeCare.Domain.Exceptions;


namespace EyeCare.Domain.Entities
{
    public class Doctor : BaseEntity
    {
        private readonly List<DoctorBranch> _doctorBranches = [];

        private Doctor()
        {
        }

        public Doctor(
            string firstName,
            string lastName,
            string phoneNumber,
            string specialty)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("Doctor first name is required.");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("Doctor last name is required.");

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new DomainException("Doctor phone number is required.");

            if (string.IsNullOrWhiteSpace(specialty))
                throw new DomainException("Doctor specialty is required.");

            Id = Guid.NewGuid();

            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Specialty = specialty;
            IsActive = true;

            CreatedAt = DateTime.UtcNow;
        }

        public string FirstName { get; private set; } = null!;

        public string LastName { get; private set; } = null!;

        public string PhoneNumber { get; private set; } = null!;

        public string Specialty { get; private set; } = null!;

        public bool IsActive { get; private set; }

        public IReadOnlyCollection<DoctorBranch> DoctorBranches =>
            _doctorBranches.AsReadOnly();

        public void UpdateInformation(
            string firstName,
            string lastName,
            string phoneNumber,
            string specialty)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("Doctor first name is required.");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("Doctor last name is required.");

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new DomainException("Doctor phone number is required.");

            if (string.IsNullOrWhiteSpace(specialty))
                throw new DomainException("Doctor specialty is required.");

            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Specialty = specialty;

            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            if (IsActive)
                return;
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            if (!IsActive)
                return;
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
        public void AssignToBranch(DoctorBranch doctorBranch)
        {
            if (doctorBranch is null)
                throw new DomainException("Doctor branch is required.");

            if (doctorBranch.DoctorId != Id)
                throw new DomainException(
                    "Doctor branch does not belong to this doctor.");

            var existingDoctorBranch = _doctorBranches
                .FirstOrDefault(x => x.BranchId == doctorBranch.BranchId);

            if (existingDoctorBranch is not null)
            {
                if (existingDoctorBranch.IsActive)
                    throw new DomainException(
                        "Doctor is already assigned to this branch.");

                existingDoctorBranch.Activate();

                UpdatedAt = DateTime.UtcNow;
                return;
            }

            _doctorBranches.Add(doctorBranch);

            UpdatedAt = DateTime.UtcNow;
        }
    }
}
