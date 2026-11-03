using EyeCare.Domain.Common;
using EyeCare.Domain.Enums;
using EyeCare.Domain.Exceptions;

namespace EyeCare.Domain.Entities;

public class Payment : BaseEntity
{
    private Payment()
    {
    }
    private readonly List<Refund> _refunds = [];
    private readonly List<PaymentAttempt> _attempts = [];
    public byte[] RowVersion { get; private set; } = [];
    public Payment(
        Guid invoiceId,
        decimal amount,
        PaymentMethod paymentMethod,
        string idempotencyKey)
    {
        if (invoiceId == Guid.Empty)
            throw new DomainException(
                "Invoice is required.");

        if (amount <= 0)
            throw new DomainException(
                "Payment amount must be greater than zero.");

        if (string.IsNullOrWhiteSpace(idempotencyKey))
            throw new DomainException(
                "Idempotency key is required.");

        Id = Guid.NewGuid();

        InvoiceId = invoiceId;
        Amount = amount;
        PaymentMethod = paymentMethod;
        IdempotencyKey = idempotencyKey;

        Status = PaymentStatus.Pending;

        CreatedAt = DateTime.UtcNow;
    }

    public Guid InvoiceId { get; private set; }

    public decimal Amount { get; private set; }

    public PaymentMethod PaymentMethod { get; private set; }

    public PaymentStatus Status { get; private set; }

    public string IdempotencyKey { get; private set; } = null!;
    public DateTime? SucceededAt { get; private set; }
    public string? ExternalTransactionId { get; private set; }

    public Invoice Invoice { get; private set; } = null!;
    public IReadOnlyCollection<Refund> Refunds =>
    _refunds.AsReadOnly();
    public IReadOnlyCollection<PaymentAttempt> Attempts => _attempts.AsReadOnly();



    public void AddAttempt(PaymentAttempt attempt)
    {
        if (attempt is null)
            throw new DomainException("Only pending payments can fail.");

        if (attempt.PaymentId != Id)
            throw new DomainException(
                "Payment attempt does not belong to this payment.");

        if (_attempts.Any(x => x.Id == attempt.Id))
            throw new DomainException(
                "Payment attempt is already registered.");

        if (_attempts.Any(x => x.AttemptKey == attempt.AttemptKey))
            throw new DomainException(
                "Payment attempt key is already registered.");

        if (Status == PaymentStatus.Succeeded)
            throw new DomainException(
                "Cannot add an attempt to a completed payment.");

        if (_attempts.Any(x => x.Status == PaymentAttemptStatus.Succeeded))
            throw new DomainException(
                "Cannot add a new attempt after a successful attempt.");

        if (_attempts.Any(x => x.Status == PaymentAttemptStatus.Unknown))
            throw new DomainException(
                "Cannot add a new attempt while an unknown attempt exists.");

        if (_attempts.Any(x => x.Status == PaymentAttemptStatus.Started))
            throw new DomainException(
                "Cannot add a new attempt while another attempt is active.");

        _attempts.Add(attempt);
    }

    public void MarkAsSucceeded(
        string externalTransactionId)
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException(
                "Only pending payments can succeed.");

        if (string.IsNullOrWhiteSpace(
            externalTransactionId))
        {
            throw new DomainException(
                "External transaction id is required.");
        }

        Status = PaymentStatus.Succeeded;
        ExternalTransactionId =
            externalTransactionId;
        SucceededAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed()
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException(
                "Only pending payments can fail.");

        Status = PaymentStatus.Failed;

        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsCancelled()
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException(
                "Only pending payments can be cancelled.");

        Status = PaymentStatus.Cancelled;

        UpdatedAt = DateTime.UtcNow;
    }

    public bool MatchesRequest(
      Guid invoiceId,
      decimal amount,
      PaymentMethod paymentMethod)
    {
        return InvoiceId == invoiceId
               && Amount == amount
               && PaymentMethod == paymentMethod;
    }


    public void RegisterRefund(Refund refund)
    {
        if (refund is null)
            throw new DomainException(
                "Refund is required.");

        if (refund.PaymentId != Id)
            throw new DomainException(
                "Refund does not belong to this payment.");

        if (_refunds.Any(x => x.Id == refund.Id))
            throw new DomainException(
                "Refund is already registered for this payment.");

        if (_refunds.Any(x => x.IdempotencyKey == refund.IdempotencyKey))
            throw new DomainException(
                "Refund idempotency key is already registered.");

        if (Status != PaymentStatus.Succeeded)
        {
            throw new DomainException(
                "Only successful payments can be refunded.");
        }

        var committedRefundAmount = _refunds
     .Where(x =>
         x.Status == RefundStatus.Pending ||
         x.Status == RefundStatus.Unknown ||
         x.Status == RefundStatus.Succeeded)
     .Sum(x => x.Amount);

        var remainingRefundable = Amount - committedRefundAmount;

        if (refund.Amount > remainingRefundable)
        {
            throw new DomainException(
                "Refund amount exceeds the refundable amount.");
        }

        _refunds.Add(refund);

        UpdatedAt = DateTime.UtcNow;
    }

    public PaymentAttempt? GetActiveAttempt()
    {
        return Attempts.FirstOrDefault(x => x.Status == Domain.Enums.PaymentAttemptStatus.Unknown)
            ?? Attempts.FirstOrDefault(x => x.Status == Domain.Enums.PaymentAttemptStatus.Started);
    }
}