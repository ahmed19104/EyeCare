using System.Collections.Concurrent;
using EyeCare.Application.Abstractions.Payments;
using EyeCare.Application.Abstractions.Payments.Contract;

namespace EyeCare.Infrastructure.Payments;

public sealed class FakePaymentGateway
    : IPaymentGateway
{
    private readonly ConcurrentDictionary<
        string,
        PaymentGatewayResult> _processedPayments = new();


    private readonly ConcurrentDictionary<
    string,
    PaymentGatewayResult> _processedRefunds = new();

    public PaymentGatewayResultStatus NextStatus { get; set; }
        = PaymentGatewayResultStatus.Succeeded;

    public bool SimulateTimeoutAfterProcessing { get; set; }= true;

    public PaymentGatewayResultStatus NextRefundStatus { get; set; }
    = PaymentGatewayResultStatus.Succeeded;

    public bool SimulateRefundTimeoutAfterProcessing { get; set; } = true;

    public Task<PaymentGatewayResult> ChargeAsync(
        PaymentGatewayRequest request,
        CancellationToken cancellationToken = default)
    {
        if (_processedPayments.TryGetValue(
                request.IdempotencyKey,
                out var existingResult))
        {
            return Task.FromResult(existingResult);
        }

        PaymentGatewayResult result;

        switch (NextStatus)
        {
            case PaymentGatewayResultStatus.Succeeded:

                result = new PaymentGatewayResult(
                    PaymentGatewayResultStatus.Succeeded,
                    Guid.NewGuid().ToString());

                break;

            case PaymentGatewayResultStatus.Failed:

                result = new PaymentGatewayResult(
                    PaymentGatewayResultStatus.Failed,
                    ErrorMessage: "Card was declined.");

                break;

            case PaymentGatewayResultStatus.Unknown:

                result = new PaymentGatewayResult(
                    PaymentGatewayResultStatus.Unknown,
                    ErrorMessage:
                        "Gateway could not determine the result.");

                break;

            default:
                throw new InvalidOperationException(
                    "Unsupported payment gateway status.");
        }

        // Simulate that the gateway processed the payment
        // before our API lost the response.
        _processedPayments.TryAdd(
            request.IdempotencyKey,
            result);

        if (SimulateTimeoutAfterProcessing)
        {
            throw new OperationCanceledException();
        }

        return Task.FromResult(result);
    }

    public Task<PaymentGatewayStatusResult> GetPaymentStatusAsync(
        string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        if (!_processedPayments.TryGetValue(
                idempotencyKey,
                out var existingResult))
        {
            return Task.FromResult(
                new PaymentGatewayStatusResult(
                    PaymentGatewayResultStatus.Unknown,
                    ErrorMessage:
                        "Payment was not found at the gateway."));
        }

        return Task.FromResult(
            new PaymentGatewayStatusResult(
                existingResult.Status,
                existingResult.ExternalTransactionId,
                existingResult.ErrorMessage));
    }


    public Task<PaymentGatewayResult> RefundAsync(
    PaymentGatewayRefundRequest request,
    CancellationToken cancellationToken = default)
    {
        if (_processedRefunds.TryGetValue(
                request.IdempotencyKey,
                out var existingResult))
        {
            return Task.FromResult(existingResult);
        }

        PaymentGatewayResult result;

        switch (NextRefundStatus)
        {
            case PaymentGatewayResultStatus.Succeeded:

                result = new PaymentGatewayResult(
                    PaymentGatewayResultStatus.Succeeded,
                    Guid.NewGuid().ToString());

                break;

            case PaymentGatewayResultStatus.Failed:

                result = new PaymentGatewayResult(
                    PaymentGatewayResultStatus.Failed,
                    ErrorMessage:
                        "Gateway rejected the refund.");

                break;

            case PaymentGatewayResultStatus.Unknown:

                result = new PaymentGatewayResult(
                    PaymentGatewayResultStatus.Unknown,
                    ErrorMessage:
                        "Gateway could not determine the refund result.");

                break;

            default:
                throw new InvalidOperationException(
                    "Unsupported refund gateway status.");
        }

        _processedRefunds.TryAdd(
            request.IdempotencyKey,
            result);

        if (SimulateRefundTimeoutAfterProcessing)
        {
            throw new OperationCanceledException();
        }

        return Task.FromResult(result);
    }

    public Task<PaymentGatewayStatusResult> GetRefundStatusAsync(
    string idempotencyKey,
    CancellationToken cancellationToken = default)
    {
        if (!_processedRefunds.TryGetValue(
                idempotencyKey,
                out var result))
        {
            return Task.FromResult(
                new PaymentGatewayStatusResult(
                    PaymentGatewayResultStatus.Unknown,
                    ErrorMessage: "Refund status is unknown."));
        }

        return Task.FromResult(
            new PaymentGatewayStatusResult(
                result.Status,
                result.ExternalTransactionId,
                result.ErrorMessage));
    }
}