using EyeCare.Domain.Enums;

namespace EyeCare.Application.Billing.Payments.GetPayment;

public sealed record GetPaymentResult(
    Guid PaymentId,
    Guid InvoiceId,
    decimal Amount,
    PaymentMethod PaymentMethod,
    PaymentStatus Status,
    string IdempotencyKey,
    string? ExternalTransactionId,
    IReadOnlyList<PaymentAttemptResult> Attempts,
    IReadOnlyList<RefundResult> Refunds);

public sealed record PaymentAttemptResult(
    Guid AttemptId,
    string AttemptKey,
    PaymentAttemptStatus Status,
    string? ExternalTransactionId,
    string? FailureReason,
    DateTime? CompletedAt);

public sealed record RefundResult(
    Guid RefundId,
    decimal Amount,
    RefundStatus Status,
    string IdempotencyKey,
    string? ExternalTransactionId);