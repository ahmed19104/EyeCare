namespace EyeCareAPI.Contracts.Doctors
{
    public sealed record CreateDoctorRequest(
      string FirstName,
      string LastName,
      string PhoneNumber,
      string Specialty);
}
