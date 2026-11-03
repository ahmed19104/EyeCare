using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;
using MediatR;

namespace EyeCare.Application.Patients.Commands.CreatePatient;

public sealed class CreatePatientCommandHandler
    : IRequestHandler<CreatePatientCommand, Guid>
{
    private readonly IRepository<Patient> _patientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePatientCommandHandler(
        IRepository<Patient> patientRepository,
        IUnitOfWork unitOfWork)
    {
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreatePatientCommand request,
        CancellationToken cancellationToken)
    {
        var patient = new Patient(
            request.FirstName,
            request.LastName,
            request.DateOfBirth,
            request.Gender,
            request.PhoneNumber,
            request.Email,
            request.NationalId);

        await _patientRepository.AddAsync(
            patient,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return patient.Id;
    }
}