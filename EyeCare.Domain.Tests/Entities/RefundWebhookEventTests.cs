
using EyeCare.Domain.Entities;
using EyeCare.Domain.Exceptions;
using Xunit;

namespace EyeCare.Domain.Tests.Entities;

public class RefundWebhookEventTests
{
    private static RefundWebhookEvent CreateEvent()
    {
        return new RefundWebhookEvent(
            $"event-{Guid.NewGuid():N}",
            "refund.succeeded",
            $"refund-key-{Guid.NewGuid():N}");
    }

    // Constructor

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenEventIdIsInvalid(
        string? eventId)
    {
        var act = () => new RefundWebhookEvent(
            eventId!, "refund.succeeded", "key-001");

        Assert.Throws<DomainException>(act);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenEventTypeIsInvalid(
        string? eventType)
    {
        var act = () => new RefundWebhookEvent(
            "event-001", eventType!, "key-002");

        Assert.Throws<DomainException>(act);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenIdempotencyKeyIsInvalid(
        string? idempotencyKey)
    {
        var act = () => new RefundWebhookEvent(
            "event-002", "refund.succeeded", idempotencyKey!);

        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Constructor_ShouldInitializeEventAsUnprocessed()
    {
        var webhookEvent = CreateEvent();

        Assert.NotEqual(Guid.Empty, webhookEvent.Id);
        Assert.False(webhookEvent.IsProcessed);
        Assert.Null(webhookEvent.ProcessedAt);
        Assert.Null(webhookEvent.ExternalTransactionId);
        Assert.Null(webhookEvent.FailureReason);
        Assert.NotEqual(default, webhookEvent.ReceivedAt);
    }

    // SetExternalTransactionId

    [Fact]
    public void SetExternalTransactionId_ShouldSetTransactionId()
    {
        var webhookEvent = CreateEvent();

        webhookEvent.SetExternalTransactionId("gateway-refund-001");

        Assert.Equal(
            "gateway-refund-001",
            webhookEvent.ExternalTransactionId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetExternalTransactionId_ShouldThrow_WhenIdIsInvalid(
        string? transactionId)
    {
        var webhookEvent = CreateEvent();

        var act = () =>
            webhookEvent.SetExternalTransactionId(transactionId!);

        Assert.Throws<DomainException>(act);
    }

    // MarkAsProcessed

    [Fact]
    public void MarkAsProcessed_ShouldSetProcessedState()
    {
        var webhookEvent = CreateEvent();

        webhookEvent.MarkAsProcessed();

        Assert.True(webhookEvent.IsProcessed);
        Assert.NotNull(webhookEvent.ProcessedAt);
        Assert.Null(webhookEvent.FailureReason);
    }

    [Fact]
    public void MarkAsProcessed_ShouldClearPreviousFailureReason()
    {
        var webhookEvent = CreateEvent();
        webhookEvent.MarkAsFailed("Temporary processing error");

        webhookEvent.MarkAsProcessed();

        Assert.True(webhookEvent.IsProcessed);
        Assert.Null(webhookEvent.FailureReason);
        Assert.NotNull(webhookEvent.ProcessedAt);
    }

    // MarkAsFailed

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void MarkAsFailed_ShouldThrow_WhenReasonIsInvalid(
        string? reason)
    {
        var webhookEvent = CreateEvent();

        var act = () => webhookEvent.MarkAsFailed(reason!);

        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void MarkAsFailed_ShouldStoreFailureReason()
    {
        var webhookEvent = CreateEvent();

        webhookEvent.MarkAsFailed("Database temporarily unavailable");

        Assert.False(webhookEvent.IsProcessed);
        Assert.Equal(
            "Database temporarily unavailable",
            webhookEvent.FailureReason);
    }

    // UpdatedAt

    [Fact]
    public void MarkAsProcessed_ShouldUpdateUpdatedAt()
    {
        var webhookEvent = CreateEvent();
        var before = DateTime.UtcNow;

        webhookEvent.MarkAsProcessed();

        Assert.NotNull(webhookEvent.UpdatedAt);
        Assert.True(webhookEvent.UpdatedAt >= before);
    }

    [Fact]
    public void MarkAsFailed_ShouldUpdateUpdatedAt()
    {
        var webhookEvent = CreateEvent();
        var before = DateTime.UtcNow;

        webhookEvent.MarkAsFailed("Processing failed");

        Assert.NotNull(webhookEvent.UpdatedAt);
        Assert.True(webhookEvent.UpdatedAt >= before);
    }
}
