using EyeCare.Application.Billing.Payments.Refunds.Webhook.ProcessRefundWebhook;
using FluentValidation;

namespace EyeCare.Application.Billing.Payments.Refunds.Webhook;

public sealed class ProcessRefundWebhookCommandValidator
    : AbstractValidator<ProcessRefundWebhookCommand>
{
    public ProcessRefundWebhookCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.EventType)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.IdempotencyKey)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Amount)
            .GreaterThan(0);

        RuleFor(x => x.ExternalTransactionId)
            .NotEmpty()
            .When(x =>
                x.EventType == "refund.succeeded");
    }
}
