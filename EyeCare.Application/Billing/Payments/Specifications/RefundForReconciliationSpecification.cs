using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;

using System.Linq.Expressions;


namespace EyeCare.Application.Billing.Payments.Specifications
{
    public sealed class RefundForReconciliationSpecification
    : ISpecification<Refund>
    {
        public RefundForReconciliationSpecification(Guid refundId)
        {
            Criteria = x => x.Id == refundId;
        }

        public Expression<Func<Refund, bool>> Criteria { get; }

        public List<Expression<Func<Refund, object>>> Includes =>
        [
            x => x.Payment,
        x => x.Attempts
        ];

        public Expression<Func<Refund, object>>? OrderByDescending
            => null;
    }
}
