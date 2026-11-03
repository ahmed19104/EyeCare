using EyeCare.Application.Abstractions.Caching;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Exceptions;
using EyeCare.Application.Patients.Commands.UpdatePatient.Specifications;
using EyeCare.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EyeCare.Application.Patients.Commands.UpdatePatient;

public sealed class UpdatePatientCommandHandler
    : IRequestHandler<UpdatePatientCommand>
{
    private readonly IRepository<Patient> _patientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;
    private readonly ILogger<UpdatePatientCommandHandler> _logger;
    public UpdatePatientCommandHandler(
        IRepository<Patient> patientRepository,
        IUnitOfWork unitOfWork,
        ICacheService cache,
        ILogger<UpdatePatientCommandHandler> logger)
    {
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
        _cache = cache;
        _logger = logger;
    }

    public async Task Handle(
        UpdatePatientCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
"Updating patient {PatientId}.",
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
    

        patient.UpdateInformation(
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Email);
        
        _logger.LogInformation(
    "Patient {PatientId} updated successfully.",
    request.PatientId);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        await _cache.RemoveAsync(
    CacheKeys.Patient(request.PatientId),
    cancellationToken);
        _logger.LogInformation("Patient {PatientId} cache removed successfully.", request.PatientId);
    }
}
