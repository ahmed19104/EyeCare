using EyeCare.Domain.Enums;

namespace EyeCareAPI.Contracts.Patients
{
   

    public sealed record CreatePatientRequest(
        string FirstName,
        string LastName,
        DateTime DateOfBirth,
        Gender Gender,
        string PhoneNumber,
        string? Email,
        string? NationalId);
}
