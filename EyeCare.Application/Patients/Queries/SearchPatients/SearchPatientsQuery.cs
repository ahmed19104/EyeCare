using MediatR;

namespace EyeCare.Application.Patients.Queries.SearchPatients;

public sealed record SearchPatientsQuery(
    string? Search,
    int Page = 1,
    int PageSize = 10)
    : IRequest<PatientSearchResponse>;
