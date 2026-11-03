namespace EyeCare.Application.Patients.Queries.GetPatientById;

public sealed record PatientByIdResponse(
    Guid Id,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string Gender,
    string PhoneNumber,
    string? Email,
    string? NationalId,
    bool IsActive);
