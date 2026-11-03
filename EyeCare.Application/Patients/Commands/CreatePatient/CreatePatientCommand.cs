using EyeCare.Domain.Enums;
using MediatR;

namespace EyeCare.Application.Patients.Commands.CreatePatient;

public sealed record CreatePatientCommand(
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    Gender Gender,
    string PhoneNumber,
    string? Email,
    string? NationalId) : IRequest<Guid>;
