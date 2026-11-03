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
    public sealed class RefundWebhookEventByEventIdSpecification
     : ISpecification<RefundWebhookEvent>
    {
        public RefundWebhookEventByEventIdSpecification(
            string eventId)
        {
            Criteria = x => x.EventId == eventId;
        }

        public Expression<Func<RefundWebhookEvent, bool>> Criteria { get; }

        public List<Expression<Func<RefundWebhookEvent, object>>> Includes
            => [];

        public Expression<Func<RefundWebhookEvent, object>>?
            OrderByDescending => null;
    }
}
