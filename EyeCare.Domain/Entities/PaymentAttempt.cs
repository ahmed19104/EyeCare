using EyeCare.Domain.Common;
using EyeCare.Domain.Enums;
using EyeCare.Domain.Exceptions;

namespace EyeCare.Domain.Entities;

public class PaymentAttempt : BaseEntity
{
    private PaymentAttempt()
    {
    }

    public PaymentAttempt(
        Guid paymentId,
        string attemptKey)
    {
        if (paymentId == Guid.Empty)
            throw new DomainException(
                "Payment is required.");

        if (string.IsNullOrWhiteSpace(attemptKey))
            throw new DomainException(
                "Attempt key is required.");

        Id = Guid.NewGuid();

        PaymentId = paymentId;
        AttemptKey = attemptKey;

        Status = PaymentAttemptStatus.Started;

        CreatedAt = DateTime.UtcNow;
    }

    public Guid PaymentId { get; private set; }

    public string AttemptKey { get; private set; } = null!;

    public PaymentAttemptStatus Status { get; private set; }

    public string? ExternalTransactionId
    {
        get;
        private set;
    }

    public string? FailureReason
    {
        get;
        private set;
    }

    public DateTime? CompletedAt
    {
        get;
        private set;
    }

    public Payment Payment { get; private set; } = null!;

    public void MarkAsSucceeded(
        string externalTransactionId)
    {
        if (Status != PaymentAttemptStatus.Started && Status != PaymentAttemptStatus.Unknown)
            throw new DomainException(
                "Only started or unknown attempts can succeed.");

        if (string.IsNullOrWhiteSpace(
            externalTransactionId))
        {
            throw new DomainException(
                "External transaction id is required.");
        }

        Status = PaymentAttemptStatus.Succeeded;
        FailureReason = null;
        ExternalTransactionId =
            externalTransactionId;

        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(
        string reason)
    {
        if (Status != PaymentAttemptStatus.Started)
            throw new DomainException(
                "Only started attempts can fail.");

        Status = PaymentAttemptStatus.Failed;

        FailureReason = reason;
        ExternalTransactionId = null;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsUnknown(
        string reason)
    {
        if (Status != PaymentAttemptStatus.Started)
            throw new DomainException(
                "Only started attempts can become unknown.");

        Status = PaymentAttemptStatus.Unknown;

        FailureReason = reason;

        UpdatedAt = DateTime.UtcNow;
    }
}