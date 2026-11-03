namespace EyeCare.Application.Abstractions.Payments.Contract;

public sealed record PaymentGatewayStatusResult(
    PaymentGatewayResultStatus Status,
    string? ExternalTransactionId = null,
    string? ErrorMessage = null);