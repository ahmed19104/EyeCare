using EyeCare.Domain.Enums;

namespace EyeCare.Application.Billing.Payments.Reconciliation;

public sealed record ReconcilePaymentResult(
    Guid PaymentId,
    PaymentStatus PaymentStatus,
    PaymentAttemptStatus AttemptStatus,
    string? ExternalTransactionId);