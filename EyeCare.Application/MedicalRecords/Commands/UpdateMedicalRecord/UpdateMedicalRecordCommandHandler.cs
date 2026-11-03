using EyeCare.Application.Abstractions.Authorization;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Exceptions;
using EyeCare.Application.MedicalRecords.Specifications;
using EyeCare.Domain.Entities;
using MediatR;

namespace EyeCare.Application.MedicalRecords.Commands.UpdateMedicalRecord;

public sealed class UpdateMedicalRecordCommandHandler
    : IRequestHandler<UpdateMedicalRecordCommand>
{
    private readonly IRepository<MedicalRecord>
        _medicalRecordRepository;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IResourceAuthorizationService
    _resourceAuthorizationService;

    public UpdateMedicalRecordCommandHandler(
        IRepository<MedicalRecord> medicalRecordRepository,
        IUnitOfWork unitOfWork, IResourceAuthorizationService resourceAuthorizationService)
    {
        _medicalRecordRepository =
            medicalRecordRepository;

        _unitOfWork = unitOfWork;
        _resourceAuthorizationService = resourceAuthorizationService;
    }

    public async Task Handle(
        UpdateMedicalRecordCommand request,
        CancellationToken cancellationToken)
    {
        var medicalRecord =
            await _medicalRecordRepository.FirstOrDefaultAsync(
                new MedicalRecordByIdSpecification(
                    request.MedicalRecordId),
                cancellationToken);

        if (medicalRecord is null)
        {
            throw new NotFoundException(
                "Medical record",
                request.MedicalRecordId);
        }
        await _resourceAuthorizationService
    .EnsureCanAccessMedicalRecordAsync(
        request.MedicalRecordId,
        cancellationToken);

        medicalRecord.Update(
            request.Diagnosis,
            request.Notes,
            request.TreatmentPlan,
            request.FollowUpDate);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}
