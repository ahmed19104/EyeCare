
using EyeCare.Application.Abstractions.Caching;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Exceptions;
using EyeCare.Application.Patients.Commands.DeactivatePatient.Specifications;
using EyeCare.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EyeCare.Application.Patients.Commands.DeactivatePatient;

public sealed class DeactivatePatientCommandHandler
    : IRequestHandler<DeactivatePatientCommand>
{
    private readonly IRepository<Patient> _patientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;
    private readonly ILogger<DeactivatePatientCommandHandler> _logger;

    public DeactivatePatientCommandHandler(
        IRepository<Patient> patientRepository,
        IUnitOfWork unitOfWork,
        ICacheService cache,
        ILogger<DeactivatePatientCommandHandler> logger)
    {
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
        _cache = cache;
        _logger = logger;
    }

    public async Task Handle(
        DeactivatePatientCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Deactivating patient {PatientId}.",
            request.PatientId);

        var patient =
            await _patientRepository.FirstOrDefaultAsync(
                new PatientByIdSpecification(
                    request.PatientId),
                cancellationToken);

        if (patient is null)
        {
            _logger.LogWarning(
                "Patient {PatientId} was not found.",
                request.PatientId);

            throw new NotFoundException(
                "Patient",
                request.PatientId);
        }

        patient.Deactivate();

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "Patient {PatientId} deactivated successfully.",
            request.PatientId);

        await _cache.RemoveAsync(
            CacheKeys.Patient(request.PatientId),
            cancellationToken);

        _logger.LogInformation(
            "Cache invalidated for patient {PatientId}.",
            request.PatientId);
    }
}

