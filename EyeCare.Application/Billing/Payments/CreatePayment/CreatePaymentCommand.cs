using EyeCare.Domain.Enums;
using MediatR;

namespace EyeCare.Application.Billing.Payments.CreatePayment;

public sealed record CreatePaymentCommand(
    Guid InvoiceId,
    decimal Amount,
    PaymentMethod PaymentMethod,
    string IdempotencyKey
) : IRequest<CreatePaymentResult>;