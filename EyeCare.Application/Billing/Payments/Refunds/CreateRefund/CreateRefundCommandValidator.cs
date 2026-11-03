using FluentValidation;

namespace EyeCare.Application.Billing.Payments.Refunds.CreateRefund;

public sealed class ReconcileRefundCommandValidator
    : AbstractValidator<CreateRefundCommand>
{
    public ReconcileRefundCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .NotEmpty()
            .WithMessage("Payment is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage(
                "Refund amount must be greater than zero.");

        RuleFor(x => x.IdempotencyKey)
            .NotEmpty()
            .WithMessage(
                "Idempotency key is required.")
            .MaximumLength(200)
            .WithMessage(
                "Idempotency key cannot exceed 200 characters.");
    }
}
