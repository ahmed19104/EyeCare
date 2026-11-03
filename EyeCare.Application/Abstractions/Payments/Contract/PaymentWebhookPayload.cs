namespace EyeCare.Application.Abstractions.Payments.Contract;

public sealed record PaymentWebhookPayload(
    string EventId,
    string EventType,
    string IdempotencyKey,
    string? ExternalTransactionId,
    decimal Amount);
