using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;
using System.Linq.Expressions;

namespace EyeCare.Application.Billing.Payments.Specifications;

public sealed class PaymentAttemptsByPaymentIdSpecification
    : ISpecification<PaymentAttempt>
{
    public PaymentAttemptsByPaymentIdSpecification(
        Guid paymentId)
    {
        Criteria = x => x.PaymentId == paymentId;
    }

    public Expression<Func<PaymentAttempt, bool>> Criteria { get; }

    public List<Expression<Func<PaymentAttempt, object>>> Includes
        => [];

    public Expression<Func<PaymentAttempt, object>>? OrderByDescending
        => null;
}