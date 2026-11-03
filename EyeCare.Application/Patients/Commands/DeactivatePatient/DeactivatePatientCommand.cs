using MediatR;

namespace EyeCare.Application.Patients.Commands.DeactivatePatient;

public sealed record DeactivatePatientCommand(
    Guid PatientId) : IRequest;