using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Patients.Queries.SearchPatients.Specifications;
using EyeCare.Domain.Entities;
using MediatR;

namespace EyeCare.Application.Patients.Queries.SearchPatients;

public sealed class SearchPatientsQueryHandler
    : IRequestHandler<
        SearchPatientsQuery,
        PatientSearchResponse>
{
    private readonly IRepository<Patient> _patientRepository;

    public SearchPatientsQueryHandler(
        IRepository<Patient> patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<PatientSearchResponse> Handle(
        SearchPatientsQuery request,
        CancellationToken cancellationToken)
    {
        var result =
            await _patientRepository.ListPagedAsync(
                new SearchPatientsSpecification(
                    request.Search),
                request.Page,
                request.PageSize,
                cancellationToken);

        var items = result.Items
            .Select(patient =>
                new PatientListItem(
                    patient.Id,
                    $"{patient.FirstName} {patient.LastName}",
                    patient.DateOfBirth,
                    patient.Gender.ToString(),
                    patient.PhoneNumber,
                    patient.NationalId))
            .ToList();

        return new PatientSearchResponse(
            items,
            result.Page,
            result.PageSize,
            result.TotalCount,
            result.TotalPages);
    }
}
