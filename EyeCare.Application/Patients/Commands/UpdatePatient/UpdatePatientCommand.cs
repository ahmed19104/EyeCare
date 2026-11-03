using MediatR;

namespace EyeCare.Application.Patients.Commands.UpdatePatient;

public sealed record UpdatePatientCommand(
    Guid PatientId,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string? Email) : IRequest;
