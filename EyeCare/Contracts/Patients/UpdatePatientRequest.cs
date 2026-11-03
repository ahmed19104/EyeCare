namespace EyeCareAPI.Contracts.Patients;

public sealed record UpdatePatientRequest(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string? Email);
