namespace EyeCare.Application.Abstractions.Payments.Contract;

public sealed record PaymentGatewayRequest(
    decimal Amount,
    string Currency,
    string IdempotencyKey);