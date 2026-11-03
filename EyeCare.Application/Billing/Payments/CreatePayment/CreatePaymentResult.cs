using EyeCare.Domain.Enums;

namespace EyeCare.Application.Billing.Payments.CreatePayment;

public sealed record CreatePaymentResult(
    Guid PaymentId,
    PaymentStatus Status,
    decimal Amount,
    string? ExternalTransactionId);