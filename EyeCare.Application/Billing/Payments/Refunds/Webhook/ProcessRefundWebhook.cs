using EyeCare.Application.Abstractions.Payments;
using EyeCare.Application.Abstractions.Payments.Contract;
using MediatR;

namespace EyeCare.Application.Billing.Payments.Refunds.Webhook.ProcessRefundWebhook;

public sealed record ProcessRefundWebhookCommand(
    string EventId,
    string EventType,
    string IdempotencyKey,
    string? ExternalTransactionId,
    decimal Amount)
    : IRequest;
