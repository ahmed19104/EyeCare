using EyeCare.Domain.Entities;

namespace EyeCare.Application.Abstractions.Billing;

public interface IRefundPolicy
{
    void EnsureCanRefund(
        Payment payment,
        decimal amount,
        DateTime utcNow);
}
