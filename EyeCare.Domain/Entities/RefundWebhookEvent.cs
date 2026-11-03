using EyeCare.Domain.Common;
using EyeCare.Domain.Exceptions;

namespace EyeCare.Domain.Entities;

public class RefundWebhookEvent : BaseEntity
{
    private RefundWebhookEvent()
    {
    }

    public RefundWebhookEvent(
        string eventId,
        string eventType,
        string idempotencyKey)
    {
        if (string.IsNullOrWhiteSpace(eventId))
            throw new DomainException(
                "Webhook event id is required.");

        if (string.IsNullOrWhiteSpace(eventType))
            throw new DomainException(
                "Webhook event type is required.");

        if (string.IsNullOrWhiteSpace(idempotencyKey))
            throw new DomainException(
                "Webhook idempotency key is required.");

        Id = Guid.NewGuid();

        EventId = eventId;
        EventType = eventType;
        IdempotencyKey = idempotencyKey;

        ReceivedAt = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }

    public string EventId { get; private set; } = null!;

    public string EventType { get; private set; } = null!;

    public string IdempotencyKey { get; private set; } = null!;

    public string? ExternalTransactionId { get; private set; }

    public DateTime ReceivedAt { get; private set; }

    public DateTime? ProcessedAt { get; private set; }

    public bool IsProcessed { get; private set; }

    public string? FailureReason { get; private set; }

    public void SetExternalTransactionId(
        string externalTransactionId)
    {
        if (string.IsNullOrWhiteSpace(
                externalTransactionId))
        {
            throw new DomainException(
                "External transaction id is required.");
        }

        ExternalTransactionId =
            externalTransactionId;
    }

    public void MarkAsProcessed()
    {
        IsProcessed = true;
        ProcessedAt = DateTime.UtcNow;
        FailureReason = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException(
                "Webhook failure reason is required.");

        IsProcessed = false;
        FailureReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }
}