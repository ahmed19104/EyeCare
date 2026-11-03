using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;
using System.Linq.Expressions;

namespace EyeCare.Application.Billing.Payments.Specifications;

public sealed class PaymentWebhookEventByEventIdSpecification
    : ISpecification<PaymentWebhookEvent>
{
    public PaymentWebhookEventByEventIdSpecification(
        string eventId)
    {
        Criteria =
            x => x.EventId == eventId;
    }

    public Expression<Func<PaymentWebhookEvent, bool>>
        Criteria
    { get; }

    public List<Expression<Func<PaymentWebhookEvent, object>>>
        Includes => [];

    public Expression<Func<PaymentWebhookEvent, object>>?
        OrderByDescending => null;
}