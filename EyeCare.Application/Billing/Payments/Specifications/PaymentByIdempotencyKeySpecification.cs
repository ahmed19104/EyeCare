using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;
using System.Linq.Expressions;

namespace EyeCare.Application.Billing.Payments.Specifications;

public sealed class PaymentByIdempotencyKeySpecification
    : ISpecification<Payment>
{
    public PaymentByIdempotencyKeySpecification(string idempotencyKey)
    {
        Criteria = x => x.IdempotencyKey == idempotencyKey;
    }

    public Expression<Func<Payment, bool>> Criteria { get; }

    public List<Expression<Func<Payment, object>>> Includes =>
    [
        x => x.Refunds
    ];

    public Expression<Func<Payment, object>>? OrderByDescending => null;
}