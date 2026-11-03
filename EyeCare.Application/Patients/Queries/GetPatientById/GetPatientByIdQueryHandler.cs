using EyeCare.Application.Abstractions.Authorization;
using EyeCare.Application.Abstractions.Caching;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Exceptions;
using EyeCare.Application.Patients.Queries.GetPatientById.Specifications;
using EyeCare.Domain.Entities;
using EyeCare.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EyeCare.Application.Patients.Queries.GetPatientById;

public sealed class GetPatientByIdQueryHandler
    : IRequestHandler<GetPatientByIdQuery, PatientByIdResponse>
{
    private readonly IRepository<Patient> _patientRepository;
    private readonly ICacheService _cache;
    private readonly IResourceAuthorizationService _resourceAuthorizationService;
    private readonly ILogger<GetPatientByIdQueryHandler> _logger;
    public GetPatientByIdQueryHandler(
        IRepository<Patient> patientRepository,
        ICacheService cache,
        IResourceAuthorizationService resourceAuthorizationService,
        ILogger<GetPatientByIdQueryHandler> logger)
    {
        _patientRepository = patientRepository;
        _cache = cache;
        _resourceAuthorizationService = resourceAuthorizationService;
        _logger = logger;
    }

    public async Task<PatientByIdResponse> Handle(
        GetPatientByIdQuery request,
        CancellationToken cancellationToken)
    {

        //await _resourceAuthorizationService.EnsureCanAccessPatientAsync(
        //    request.PatientId,

        //    cancellationToken);
        // 2. Cache key
        var cacheKey = CacheKeys.Patient(request.PatientId);

        // 3. Try cache
        var cachedPatient =
            await _cache.GetAsync<PatientByIdResponse>(
                cacheKey,
                cancellationToken);

        if (cachedPatient is not null)
        {
            _logger.LogInformation("Cache hit for patient {PatientId}", request.PatientId);
            return cachedPatient;
        }
        _logger.LogInformation(
    "Patient {PatientId} cache miss. Loading from database.",
    request.PatientId);

        var patient =
            await _patientRepository.FirstOrDefaultAsync(
                new PatientByIdSpecification(request.PatientId),
                cancellationToken);

        if (patient is null)
        {
            throw new NotFoundException(
                "Patient",
                request.PatientId);
        }

        var response = new PatientByIdResponse(
            patient.Id,
            patient.FirstName,
            patient.LastName,
            patient.DateOfBirth,
            patient.Gender.ToString(),
            patient.PhoneNumber,
            patient.Email,
            patient.NationalId,
            patient.IsActive);

        // 4. Set cache
        await _cache.SetAsync(
       cacheKey,
       response,
       TimeSpan.FromMinutes(5),
       cancellationToken);

        _logger.LogInformation("Patient {PatientId} cached successfully.", request.PatientId);

        return response;
    }
}
