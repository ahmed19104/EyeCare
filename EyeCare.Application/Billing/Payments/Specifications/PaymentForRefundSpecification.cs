using System.Linq.Expressions;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;

namespace EyeCare.Application.Billing.Payments.Specifications;

public sealed class PaymentForRefundSpecification
    : ISpecification<Payment>
{
    public PaymentForRefundSpecification(
        Guid paymentId)
    {
        Criteria = x => x.Id == paymentId;
    }

    public Expression<Func<Payment, bool>> Criteria { get; }

    public List<Expression<Func<Payment, object>>> Includes =>
    [
        x => x.Refunds,
        x => x.Invoice
    ];

    public Expression<Func<Payment, object>>?
        OrderByDescending => null;
}
