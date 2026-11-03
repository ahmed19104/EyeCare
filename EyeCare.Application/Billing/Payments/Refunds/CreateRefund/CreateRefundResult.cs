using EyeCare.Domain.Enums;

namespace EyeCare.Application.Billing.Payments.Refunds.CreateRefund;

public sealed record CreateRefundResult(
    Guid RefundId,
    Guid PaymentId,
    decimal Amount,
    RefundStatus Status,
    string? ExternalTransactionId);
