namespace EyeCare.Application.Abstractions.Payments.Contract;

public sealed record PaymentGatewayResult(
    PaymentGatewayResultStatus Status,
    string? ExternalTransactionId = null,
    string? ErrorMessage = null);