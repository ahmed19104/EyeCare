using System.Linq.Expressions;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;

namespace EyeCare.Application.Billing.Payments.Specifications;

public sealed class RefundByIdempotencyKeySpecification
    : ISpecification<Refund>
{
    public RefundByIdempotencyKeySpecification(
        string idempotencyKey)
    {
        Criteria =
            x => x.IdempotencyKey == idempotencyKey;
    }

    public Expression<Func<Refund, bool>> Criteria { get; }

    public List<Expression<Func<Refund, object>>> Includes =>
    [];

    public Expression<Func<Refund, object>>?
        OrderByDescending => null;
}
