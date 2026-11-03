using MediatR;

namespace EyeCare.Application.Patients.Queries.GetPatientById;

public sealed record GetPatientByIdQuery(
    Guid PatientId) : IRequest<PatientByIdResponse>;