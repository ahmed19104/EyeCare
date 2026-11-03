

using EyeCare.Domain.Common;
using EyeCare.Domain.Exceptions;

namespace EyeCare.Domain.Entities
{
    public class DoctorBranch : BaseEntity
    {
        private DoctorBranch()
        {
        }

        public DoctorBranch(
            Guid doctorId,
            Guid branchId)
        {
            if (doctorId == Guid.Empty)
                throw new DomainException("Doctor is required.");

            if (branchId == Guid.Empty)
                throw new DomainException("Branch is required.");

            Id = Guid.NewGuid();

            DoctorId = doctorId;
            BranchId = branchId;
            IsActive = true;

            CreatedAt = DateTime.UtcNow;
        }

        public Guid DoctorId { get; private set; }

        public Guid BranchId { get; private set; }

        public bool IsActive { get; private set; }

        public Doctor Doctor { get; private set; } = null!;

        public Branch Branch { get; private set; } = null!;

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
    }
}
