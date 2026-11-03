using EyeCare.Application.Abstractions.Authorization;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Exceptions;
using EyeCare.Application.MedicalRecords.Specifications;
using EyeCare.Domain.Entities;
using MediatR;

namespace EyeCare.Application.MedicalRecords.Queries
    .GetMedicalRecordAuditHistory;

public sealed class GetMedicalRecordAuditHistoryHandler
    : IRequestHandler<
        GetMedicalRecordAuditHistoryQuery,
        IReadOnlyList<AuditHistoryItem>>
{
    private readonly IRepository<MedicalRecord> _medicalRecordRepository;
    private readonly IRepository<AuditLog> _auditLogRepository;
    private readonly IResourceAuthorizationService
    _resourceAuthorizationService;

    public GetMedicalRecordAuditHistoryHandler(
        IRepository<MedicalRecord> medicalRecordRepository,
        IRepository<AuditLog> auditLogRepository, IResourceAuthorizationService resourceAuthorizationService)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _auditLogRepository = auditLogRepository;
        _resourceAuthorizationService = resourceAuthorizationService;
    }

    public async Task<IReadOnlyList<AuditHistoryItem>> Handle(
        GetMedicalRecordAuditHistoryQuery request,
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

        var auditLogs =
            await _auditLogRepository.ListAsync(
                new AuditLogsByEntitySpecification(
                    nameof(MedicalRecord),
                    request.MedicalRecordId),
                cancellationToken);

        return auditLogs
            .Select(auditLog =>
                new AuditHistoryItem(
                    auditLog.Id,
                    auditLog.Action.ToString(),
                    auditLog.UserId,
                    auditLog.Changes,
                    auditLog.CreatedAt))
            .ToList();
    }
}
