using EyeCare.Application.Abstractions.Payments.Contract;

namespace EyeCare.Application.Abstractions.Payments;

public interface IPaymentGateway
{
    Task<PaymentGatewayResult> ChargeAsync(
        PaymentGatewayRequest request,
        CancellationToken cancellationToken = default);

    Task<PaymentGatewayStatusResult> GetPaymentStatusAsync(
        string idempotencyKey,
        CancellationToken cancellationToken = default);

    Task<PaymentGatewayResult> RefundAsync(
      PaymentGatewayRefundRequest request,
      CancellationToken cancellationToken = default);

    Task<PaymentGatewayStatusResult> GetRefundStatusAsync(
        string idempotencyKey,
        CancellationToken cancellationToken = default);
}