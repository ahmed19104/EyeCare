
using EyeCare.Domain.Entities;
using EyeCare.Domain.Exceptions;
using Xunit;

namespace EyeCare.Domain.Tests.Entities;

public class PaymentWebhookEventTests
{
    [Fact]
    public void Constructor_ShouldCreateEvent_WithValidData()
    {
        // Arrange
        var eventId = "evt_123";
        var eventType = "payment.succeeded";
        var idempotencyKey = "payment-key-001";

        var before = DateTime.UtcNow;

        // Act
        var webhookEvent = new PaymentWebhookEvent(
            eventId,
            eventType,
            idempotencyKey);

        var after = DateTime.UtcNow;

        // Assert
        Assert.NotEqual(Guid.Empty, webhookEvent.Id);
        Assert.Equal(eventId, webhookEvent.EventId);
        Assert.Equal(eventType, webhookEvent.EventType);
        Assert.Equal(idempotencyKey, webhookEvent.IdempotencyKey);

        Assert.Null(webhookEvent.ExternalTransactionId);
        Assert.False(webhookEvent.IsProcessed);
        Assert.Null(webhookEvent.ProcessedAt);
        Assert.Null(webhookEvent.FailureReason);

        Assert.InRange(webhookEvent.ReceivedAt, before, after);
        Assert.InRange(webhookEvent.CreatedAt, before, after);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Constructor_ShouldThrow_WhenEventIdIsInvalid(
        string? eventId)
    {
        Assert.Throws<DomainException>(() =>
            new PaymentWebhookEvent(
                eventId!,
                "payment.succeeded",
                "payment-key-001"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Constructor_ShouldThrow_WhenEventTypeIsInvalid(
        string? eventType)
    {
        Assert.Throws<DomainException>(() =>
            new PaymentWebhookEvent(
                "evt_123",
                eventType!,
                "payment-key-001"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Constructor_ShouldThrow_WhenIdempotencyKeyIsInvalid(
        string? idempotencyKey)
    {
        Assert.Throws<DomainException>(() =>
            new PaymentWebhookEvent(
                "evt_123",
                "payment.succeeded",
                idempotencyKey!));
    }

    [Fact]
    public void SetExternalTransactionId_ShouldSetValue()
    {
        // Arrange
        var webhookEvent = CreateWebhookEvent();

        // Act
        webhookEvent.SetExternalTransactionId("gw-tx-123");

        // Assert
        Assert.Equal(
            "gw-tx-123",
            webhookEvent.ExternalTransactionId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void SetExternalTransactionId_ShouldThrow_WhenValueIsInvalid(
        string? externalTransactionId)
    {
        // Arrange
        var webhookEvent = CreateWebhookEvent();

        // Act & Assert
        Assert.Throws<DomainException>(() =>
            webhookEvent.SetExternalTransactionId(
                externalTransactionId!));
    }

    [Fact]
    public void SetExternalTransactionId_ShouldReplacePreviousValue()
    {
        // Arrange
        var webhookEvent = CreateWebhookEvent();
        webhookEvent.SetExternalTransactionId("gw-tx-old");

        // Act
        webhookEvent.SetExternalTransactionId("gw-tx-new");

        // Assert
        Assert.Equal(
            "gw-tx-new",
            webhookEvent.ExternalTransactionId);
    }

    [Fact]
    public void MarkAsProcessed_ShouldUpdateProcessingState()
    {
        // Arrange
        var webhookEvent = CreateWebhookEvent();
        var before = DateTime.UtcNow;

        // Act
        webhookEvent.MarkAsProcessed();

        var after = DateTime.UtcNow;

        // Assert
        Assert.True(webhookEvent.IsProcessed);
        Assert.NotNull(webhookEvent.ProcessedAt);
        Assert.Null(webhookEvent.FailureReason);

        Assert.InRange(
            webhookEvent.ProcessedAt!.Value,
            before,
            after);
        Assert.NotNull(webhookEvent.UpdatedAt);
        Assert.InRange(
            webhookEvent.UpdatedAt.Value,
            before,
            after);
    }

    [Fact]
    public void MarkAsProcessed_ShouldClearPreviousFailureReason()
    {
        // Arrange
        var webhookEvent = CreateWebhookEvent();
        webhookEvent.MarkAsFailed("Temporary processing error");

        // Act
        webhookEvent.MarkAsProcessed();

        // Assert
        Assert.True(webhookEvent.IsProcessed);
        Assert.Null(webhookEvent.FailureReason);
    }

    [Fact]
    public void MarkAsProcessed_CalledTwice_ShouldRemainProcessed()
    {
        // Arrange
        var webhookEvent = CreateWebhookEvent();
        webhookEvent.MarkAsProcessed();

        // Act
        webhookEvent.MarkAsProcessed();

        // Assert
        Assert.True(webhookEvent.IsProcessed);
        Assert.NotNull(webhookEvent.ProcessedAt);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void MarkAsFailed_ShouldThrow_WhenReasonIsInvalid(
        string? reason)
    {
        // Arrange
        var webhookEvent = CreateWebhookEvent();

        // Act & Assert
        Assert.Throws<DomainException>(() =>
            webhookEvent.MarkAsFailed(reason!));
    }

    [Fact]
    public void MarkAsFailed_ShouldRecordFailureReason()
    {
        // Arrange
        var webhookEvent = CreateWebhookEvent();
        var before = DateTime.UtcNow;
        var reason = "Gateway response could not be processed";

        // Act
        webhookEvent.MarkAsFailed(reason);

        var after = DateTime.UtcNow;

        // Assert
        Assert.False(webhookEvent.IsProcessed);
        Assert.Equal(reason, webhookEvent.FailureReason);
        Assert.Null(webhookEvent.ProcessedAt);
        Assert.NotNull(webhookEvent.UpdatedAt);
        Assert.InRange(
            webhookEvent.UpdatedAt.Value,
            before,
            after);
    }

    [Fact]
    public void MarkAsFailed_AfterProcessed_ShouldSetIsProcessedToFalse()
    {
        // Arrange
        var webhookEvent = CreateWebhookEvent();
        webhookEvent.MarkAsProcessed();

        // Act
        webhookEvent.MarkAsFailed("Processing failed");

        // Assert
        Assert.False(webhookEvent.IsProcessed);
        Assert.Equal(
            "Processing failed",
            webhookEvent.FailureReason);

        // ملاحظة: الكود الحالي لا يمسح ProcessedAt عند الفشل.
        Assert.NotNull(webhookEvent.ProcessedAt);
    }

    [Fact]
    public void MarkAsFailed_CalledAgain_ShouldReplaceFailureReason()
    {
        // Arrange
        var webhookEvent = CreateWebhookEvent();
        webhookEvent.MarkAsFailed("First failure");

        // Act
        webhookEvent.MarkAsFailed("Second failure");

        // Assert
        Assert.Equal(
            "Second failure",
            webhookEvent.FailureReason);
    }

    [Fact]
    public void Constructor_ShouldGenerateUniqueIds_ForDifferentEvents()
    {
        // Arrange & Act
        var first = CreateWebhookEvent();
        var second = CreateWebhookEvent();

        // Assert
        Assert.NotEqual(first.Id, second.Id);
    }

    private static PaymentWebhookEvent CreateWebhookEvent()
    {
        return new PaymentWebhookEvent(
            "evt_123",
            "payment.succeeded",
            "payment-key-001");
    }
}

