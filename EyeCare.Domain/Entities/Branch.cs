using EyeCare.Domain.Common;
using EyeCare.Domain.Exceptions;
using System;


namespace EyeCare.Domain.Entities
{
  
    public class Branch : BaseEntity
    {
        private readonly List<DoctorBranch> _doctorBranches = [];

        private Branch()
        {
        }

        public Branch(
            string name,
            string address,
            string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Branch name is required.");

            if (string.IsNullOrWhiteSpace(address))
                throw new DomainException("Branch address is required.");

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new DomainException("Branch phone number is required.");

            Id = Guid.NewGuid();

            Name = name;
            Address = address;
            PhoneNumber = phoneNumber;
            IsActive = true;

            CreatedAt = DateTime.UtcNow;
        }

        public string Name { get; private set; } = null!;

        public string Address { get; private set; } = null!;

        public string PhoneNumber { get; private set; } = null!;

        public bool IsActive { get; private set; }

        public IReadOnlyCollection<DoctorBranch> DoctorBranches =>
            _doctorBranches.AsReadOnly();

        public void UpdateInformation(
            string name,
            string address,
            string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Branch name is required.");

            if (string.IsNullOrWhiteSpace(address))
                throw new DomainException("Branch address is required.");

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new DomainException("Branch phone number is required.");

            Name = name;
            Address = address;
            PhoneNumber = phoneNumber;
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

        public void AddDoctor(DoctorBranch doctorBranch)
        {
            if (doctorBranch is null)
                throw new DomainException("Doctor branch is required.");

            if (doctorBranch.BranchId != Id)
                throw new DomainException(
                    "Doctor branch does not belong to this branch.");

            var existingDoctorBranch = _doctorBranches
                .FirstOrDefault(x => x.DoctorId == doctorBranch.DoctorId);

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
