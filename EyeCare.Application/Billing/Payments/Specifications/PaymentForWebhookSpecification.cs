using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;
using System.Linq.Expressions;

namespace EyeCare.Application.Billing.Payments.Specifications;

public sealed class PaymentForWebhookSpecification
    : ISpecification<Payment>
{
    public PaymentForWebhookSpecification(
        string idempotencyKey)
    {
        Criteria =
            x => x.IdempotencyKey == idempotencyKey;
    }

    public Expression<Func<Payment, bool>> Criteria { get; }

    public List<Expression<Func<Payment, object>>> Includes =>
    [
         x => x.Invoice.Items,
        x => x.Attempts
    ];
  

    public Expression<Func<Payment, object>>?
        OrderByDescending => null;
}