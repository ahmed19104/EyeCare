using EyeCare.Domain.Common;
using EyeCare.Domain.Enums;
using EyeCare.Domain.Exceptions;


namespace EyeCare.Domain.Entities
{
    public class Patient : BaseEntity
    {
        public string FirstName { get; private set; } = null!;

        public string LastName { get; private set; } = null!;

        public DateTime DateOfBirth { get; private set; }

        public Gender Gender { get; private set; }

        public string PhoneNumber { get; private set; } = null!;

        public string? Email { get; private set; }

        public string? NationalId { get; private set; }
        public bool IsActive { get; private set; }


        private Patient()
        {
        }

        public Patient(
     string firstName,
     string lastName,
     DateTime dateOfBirth,
     Gender gender,
     string phoneNumber,
     string? email,
     string? nationalId)
        {
            if (dateOfBirth.Date > DateTime.UtcNow.Date)
            {
                throw new DomainException(
                    "Patient date of birth cannot be in the future.");
            }

            Id = Guid.NewGuid();

            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            PhoneNumber = phoneNumber;
            Email = email;
            NationalId = nationalId;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }


        public void UpdateInformation(
       string firstName,
       string lastName,
       string phoneNumber,
       string? email)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new DomainException(
                    "Patient first name is required.");
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new DomainException(
                    "Patient last name is required.");
            }

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                throw new DomainException(
                    "Patient phone number is required.");
            }

            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Email = email;

            UpdatedAt = DateTime.UtcNow;
        }
        public void Deactivate()
        {
            if (!IsActive)
            {
                throw new DomainException(
                    "Patient is already inactive.");
            }

            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            if (IsActive)
            {
                throw new DomainException(
                    "Patient is already active.");
            }

            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
