using EyeCare.Domain.Enums;

namespace EyeCareAPI.Contracts.Payments;

public sealed record CreatePaymentRequest(
    Guid InvoiceId,
    decimal Amount,
    PaymentMethod PaymentMethod
    );
