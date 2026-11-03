using System.Linq.Expressions;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;

namespace EyeCare.Application.Billing.Payments.Specifications;

public sealed class PaymentForReconciliationSpecification
    : ISpecification<Payment>
{
    public PaymentForReconciliationSpecification(
        Guid paymentId)
    {
        Criteria = x => x.Id == paymentId;
    }

    public Expression<Func<Payment, bool>> Criteria { get; }

    public List<Expression<Func<Payment, object>>> Includes =>
    [
        x => x.Attempts,
        x => x.Invoice
    ];

    public Expression<Func<Payment, object>>?
        OrderByDescending => null;
}