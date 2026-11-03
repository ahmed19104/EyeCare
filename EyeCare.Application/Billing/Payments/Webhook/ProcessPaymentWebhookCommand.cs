using MediatR;

namespace EyeCare.Application.Billing.Payments.Webhook;

public sealed record ProcessPaymentWebhookCommand(
    string EventId,
    string EventType,
    string IdempotencyKey,
    string? ExternalTransactionId,
    decimal Amount
) : IRequest;
