using EyeCare.Domain.Enums;
using MediatR;

namespace EyeCare.Application.Billing.Payments.Refunds.CreateRefund;

public sealed record CreateRefundCommand(
    Guid PaymentId,
    decimal Amount,
    string IdempotencyKey)
    : IRequest<CreateRefundResult>;
