using EyeCare.Domain.Common;
using EyeCare.Domain.Enums;
using EyeCare.Domain.Exceptions;

namespace EyeCare.Domain.Entities;

public class RefundAttempt : BaseEntity
{
    private RefundAttempt()
    {
    }

    public RefundAttempt(
        Guid refundId,
        string attemptKey)
    {
        if (refundId == Guid.Empty)
            throw new DomainException(
                "Refund is required.");

        if (string.IsNullOrWhiteSpace(attemptKey))
            throw new DomainException(
                "Attempt key is required.");

        Id = Guid.NewGuid();

        RefundId = refundId;

        AttemptKey = attemptKey;

        Status = RefundAttemptStatus.Started;

        CreatedAt = DateTime.UtcNow;
    }

    public Guid RefundId { get; private set; }

    public string AttemptKey { get; private set; }
        = null!;

    public RefundAttemptStatus Status { get; private set; }

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

    public Refund Refund { get; private set; }
        = null!;

    public void MarkAsSucceeded(
        string externalTransactionId)
    {
        if (Status != RefundAttemptStatus.Started &&
            Status != RefundAttemptStatus.Unknown)
        {
            throw new DomainException(
                "Only started or unknown attempts can succeed.");
        }

        if (string.IsNullOrWhiteSpace(
                externalTransactionId))
        {
            throw new DomainException(
                "External transaction id is required.");
        }

        Status = RefundAttemptStatus.Succeeded;

        ExternalTransactionId =
            externalTransactionId;

        FailureReason = null;

        CompletedAt = DateTime.UtcNow;

        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(
        string reason)
    {
        if (Status != RefundAttemptStatus.Started &&
            Status != RefundAttemptStatus.Unknown)
        {
            throw new DomainException(
                "Only started or unknown attempts can fail.");
        }

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException(
                "Failure reason is required.");

        Status = RefundAttemptStatus.Failed;

        ExternalTransactionId = null;

        FailureReason = reason;

        CompletedAt = DateTime.UtcNow;

        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsUnknown(
        string reason)
    {
        if (Status != RefundAttemptStatus.Started)
        {
            throw new DomainException(
                "Only started attempts can become unknown.");
        }

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException(
                "Failure reason is required.");

        Status = RefundAttemptStatus.Unknown;

        FailureReason = reason;

        UpdatedAt = DateTime.UtcNow;
    }
}
