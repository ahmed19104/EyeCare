using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EyeCare.Application.Billing.Payments.Specifications
{
    public sealed class RefundForWebhookSpecification
    : ISpecification<Refund>
    {
        public RefundForWebhookSpecification(
            string idempotencyKey)
        {
            Criteria = x =>
                x.IdempotencyKey == idempotencyKey;
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
