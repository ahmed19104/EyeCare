using EyeCare.Domain.Common;
using EyeCare.Domain.Enums;
using EyeCare.Domain.Exceptions;

namespace EyeCare.Domain.Entities;

public class Refund : BaseEntity
{
    private Refund()
    {
    }
    public byte[] RowVersion { get; private set; } = [];
    private readonly List<RefundAttempt> _attempts = [];
    public Refund(
        Guid paymentId,
        decimal amount,
        string idempotencyKey)
    {
        if (paymentId == Guid.Empty)
            throw new DomainException(
                "Payment is required.");

        if (amount <= 0)
            throw new DomainException(
                "Refund amount must be greater than zero.");

        if (string.IsNullOrWhiteSpace(idempotencyKey))
            throw new DomainException(
                "Idempotency key is required.");

        Id = Guid.NewGuid();

        PaymentId = paymentId;
        Amount = amount;
        IdempotencyKey = idempotencyKey;

        Status = RefundStatus.Pending;

        CreatedAt = DateTime.UtcNow;
    }

    public Guid PaymentId { get; private set; }

    public decimal Amount { get; private set; }

    public RefundStatus Status { get; private set; }

    public string IdempotencyKey { get; private set; } = null!;

    public string? ExternalTransactionId
    {
        get;
        private set;
    }
    public IReadOnlyCollection<RefundAttempt> Attempts
    => _attempts.AsReadOnly();
    public Payment Payment { get; private set; } = null!;

    public void MarkAsSucceeded(
        string externalTransactionId)
    {
        if (Status != RefundStatus.Pending)
            throw new DomainException(
                "Only pending refunds can succeed.");

        if (string.IsNullOrWhiteSpace(
            externalTransactionId))
        {
            throw new DomainException(
                "External transaction id is required.");
        }

        Status = RefundStatus.Succeeded;

        ExternalTransactionId =
            externalTransactionId;

        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed()
    {
        if (Status != RefundStatus.Pending)
            throw new DomainException(
                "Only pending refunds can fail.");

        Status = RefundStatus.Failed;

        UpdatedAt = DateTime.UtcNow;
    }
    public void MarkAsFailed(string reason)
    {
        if (Status != RefundStatus.Pending)
            throw new DomainException(
                "Only pending refunds can fail.");

        Status = RefundStatus.Failed;

        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsUnknown()
    {
        if (Status != RefundStatus.Pending)
            throw new DomainException(
                "Only pending refunds can become unknown.");

        Status = RefundStatus.Unknown;

        UpdatedAt = DateTime.UtcNow;
    }
    public void MarkAsUnknown(string reason)
    {
        if (Status != RefundStatus.Pending)
            throw new DomainException(
                "Only pending refunds can become unknown.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException(
                "Reason is required.");

        Status = RefundStatus.Unknown;

        UpdatedAt = DateTime.UtcNow;
    }
    public void AddAttempt(
      RefundAttempt attempt)
    {
        if (attempt is null)
            throw new DomainException(
                "Refund attempt is required.");

        _attempts.Add(attempt);

        UpdatedAt = DateTime.UtcNow;
    }

    public bool MatchesRequest(
    Guid paymentId,
    decimal amount)
    {
        return PaymentId == paymentId
               && Amount == amount;
    }
    public void Cancel()
    {
        if (Status != RefundStatus.Pending)
            throw new DomainException(
                "Only pending refunds can be cancelled.");

        Status = RefundStatus.Cancelled;

        UpdatedAt = DateTime.UtcNow;
    }

    public RefundAttempt? GetActiveAttempt()
    {
        return Attempts.FirstOrDefault(x => x.Status == Domain.Enums.RefundAttemptStatus.Unknown)
            ?? Attempts.FirstOrDefault(x => x.Status == Domain.Enums.RefundAttemptStatus.Started);
    }
}