
using EyeCare.Domain.Entities;
using EyeCare.Domain.Enums;
using EyeCare.Domain.Common;
using Xunit;

namespace EyeCare.Domain.Tests.Entities;

public class AuditLogTests
{
    [Fact]
    public void Constructor_ShouldCreateAuditLog_WithValidData()
    {
        // Arrange
        var entityName = "Invoice";
        var entityId = Guid.NewGuid();
        var action = Enum.GetValues<AuditAction>()[0];
        var userId = "user-123";
        var changes = """{"Status":{"Old":"Draft","New":"Issued"}}""";

        // Act
        var auditLog = new AuditLog(
            entityName,
            entityId,
            action,
            userId,
            changes);

        // Assert
        Assert.NotEqual(Guid.Empty, auditLog.Id);
        Assert.Equal(entityName, auditLog.EntityName);
        Assert.Equal(entityId, auditLog.EntityId);
        Assert.Equal(action, auditLog.Action);
        Assert.Equal(userId, auditLog.UserId);
        Assert.Equal(changes, auditLog.Changes);
        Assert.NotEqual(default, auditLog.CreatedAt);
    }

    [Fact]
    public void Constructor_ShouldGenerateUniqueIds_ForDifferentAuditLogs()
    {
        // Arrange
        var entityId = Guid.NewGuid();

        // Act
        var first = CreateAuditLog(entityId: entityId);
        var second = CreateAuditLog(entityId: entityId);

        // Assert
        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void Constructor_ShouldAllowNullUserId()
    {
        // Arrange
        var entityId = Guid.NewGuid();

        // Act
        var auditLog = new AuditLog(
            "Invoice",
            entityId,
            Enum.GetValues<AuditAction>()[0],
            null,
            "{}");

        // Assert
        Assert.Null(auditLog.UserId);
    }

    [Fact]
    public void Constructor_ShouldPreserveEmptyUserId()
    {
        // Arrange
        var entityId = Guid.NewGuid();

        // Act
        var auditLog = new AuditLog(
            "Invoice",
            entityId,
            Enum.GetValues<AuditAction>()[0],
            "",
            "{}");

        // Assert
        Assert.Equal("", auditLog.UserId);
    }

    [Fact]
    public void Constructor_ShouldPreserveEntityName()
    {
        // Arrange
        var entityName = "  Invoice  ";

        // Act
        var auditLog = CreateAuditLog(
            entityName: entityName);

        // Assert
        Assert.Equal(entityName, auditLog.EntityName);
    }

    [Fact]
    public void Constructor_ShouldPreserveChangesExactly()
    {
        // Arrange
        var changes = """
        {
          "Amount": {
            "Old": 100,
            "New": 200
          }
        }
        """;

        // Act
        var auditLog = CreateAuditLog(changes: changes);

        // Assert
        Assert.Equal(changes, auditLog.Changes);
    }

    [Fact]
    public void Constructor_ShouldPreserveEmptyChanges()
    {
        // Act
        var auditLog = CreateAuditLog(changes: "");

        // Assert
        Assert.Equal("", auditLog.Changes);
    }

    [Fact]
    public void Constructor_ShouldPreserveWhitespaceEntityName()
    {
        // Act
        var auditLog = CreateAuditLog(entityName: "   ");

        // Assert
        Assert.Equal("   ", auditLog.EntityName);
    }

    [Fact]
    public void Constructor_ShouldPreserveEmptyEntityName()
    {
        // Act
        var auditLog = CreateAuditLog(entityName: "");

        // Assert
        Assert.Equal("", auditLog.EntityName);
    }

    [Fact]
    public void Constructor_ShouldPreserveEmptyChangesJson()
    {
        // Act
        var auditLog = CreateAuditLog(changes: "{}");

        // Assert
        Assert.Equal("{}", auditLog.Changes);
    }

    [Fact]
    public void Constructor_ShouldPreserveEntityId()
    {
        // Arrange
        var entityId = Guid.NewGuid();

        // Act
        var auditLog = CreateAuditLog(entityId: entityId);

        // Assert
        Assert.Equal(entityId, auditLog.EntityId);
    }

    [Fact]
    public void Constructor_ShouldPreserveEmptyEntityId()
    {
        // Act
        var auditLog = CreateAuditLog(entityId: Guid.Empty);

        // Assert
        Assert.Equal(Guid.Empty, auditLog.EntityId);
    }

    [Fact]
    public void Constructor_ShouldPreserveAction()
    {
        // Arrange
        var action = Enum.GetValues<AuditAction>()[^1];

        // Act
        var auditLog = CreateAuditLog(action: action);

        // Assert
        Assert.Equal(action, auditLog.Action);
    }

    private static AuditLog CreateAuditLog(
        string entityName = "Invoice",
        Guid? entityId = null,
        AuditAction? action = null,
        string? userId = "user-123",
        string changes = "{}")
    {
        return new AuditLog(
            entityName,
            entityId ?? Guid.NewGuid(),
            action ?? Enum.GetValues<AuditAction>()[0],
            userId,
            changes);
    }
}
