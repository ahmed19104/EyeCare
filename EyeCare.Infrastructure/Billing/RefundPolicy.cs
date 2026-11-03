using EyeCare.Application.Abstractions.Billing;
using EyeCare.Application.Exceptions;
using EyeCare.Domain.Entities;
using Microsoft.Extensions.Options;

namespace EyeCare.Infrastructure.Billing;

public sealed class RefundPolicy : IRefundPolicy
{
    private readonly RefundOptions _options;

    public RefundPolicy(
        IOptions<RefundOptions> options)
    {
        _options = options.Value;
    }

    public void EnsureCanRefund(
        Payment payment,
        decimal amount,
        DateTime utcNow)
    {
        if (payment.Status != Domain.Enums.PaymentStatus.Succeeded)
        {
            throw new PaymentProcessingException(
                "Only successful payments can be refunded.");
        }

        if (payment.SucceededAt is null)
        {
            throw new PaymentProcessingException(
                "Payment does not have a succeeded timestamp.");
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount));
        }

        var refundDeadline =
            payment.SucceededAt.Value
                .AddDays(_options.WindowDays);

        if (utcNow > refundDeadline)
        {
            throw new RefundWindowExpiredException();
        }
    }
}
