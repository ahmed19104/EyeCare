using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;
using System.Linq.Expressions;

namespace EyeCare.Application.Billing.Payments.Specifications;

public sealed class PaymentDetailsSpecification
    : ISpecification<Payment>
{
    public PaymentDetailsSpecification(Guid paymentId)
    {
        Criteria = x => x.Id == paymentId;
    }

    public Expression<Func<Payment, bool>> Criteria { get; }

    public List<Expression<Func<Payment, object>>> Includes =>
    [
       x => x.Refunds,
    x => x.Attempts
    ];

    public Expression<Func<Payment, object>>? OrderByDescending
        => null;
}