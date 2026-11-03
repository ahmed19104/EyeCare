using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;
using System.Linq.Expressions;

namespace EyeCare.Application.MedicalRecords.Specifications;

public sealed class AuditLogsByEntitySpecification
    : ISpecification<AuditLog>
{
    public AuditLogsByEntitySpecification(
        string entityName,
        Guid entityId)
    {
        Criteria = auditLog =>
            auditLog.EntityName == entityName &&
            auditLog.EntityId == entityId;
    }

    public Expression<Func<AuditLog, bool>> Criteria { get; }

    public List<Expression<Func<AuditLog, object>>> Includes { get; }
        = [];

    public Expression<Func<AuditLog, object>>?
        OrderByDescending
    { get; }
        = auditLog => auditLog.CreatedAt;
}
