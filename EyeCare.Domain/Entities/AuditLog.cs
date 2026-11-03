using EyeCare.Domain.Common;
using EyeCare.Domain.Enums;
using System;

namespace EyeCare.Domain.Entities;

public class AuditLog : BaseEntity
{
    private AuditLog()
    {
    }

    public AuditLog(
        string entityName,
        Guid entityId,
        AuditAction action, 
        string? userId,
        string changes)
    {
        Id = Guid.NewGuid();

        EntityName = entityName;
        EntityId = entityId;
        Action = action;
        UserId = userId;
        Changes = changes;

        CreatedAt = DateTime.UtcNow;
    }

    public string EntityName { get; private set; } = null!;

    public Guid EntityId { get; private set; }

    public AuditAction Action { get; private set; }

    public string? UserId { get; private set; }

    public string Changes { get; private set; } = null!;
}
