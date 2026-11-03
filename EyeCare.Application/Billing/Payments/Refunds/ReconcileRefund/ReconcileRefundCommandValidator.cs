using FluentValidation;

namespace EyeCare.Application.Billing.Payments.Refunds.ReconcileRefund;

public sealed class ReconcileRefundCommandValidator
    : AbstractValidator<ReconcileRefundCommand>
{
    public ReconcileRefundCommandValidator()
    {
        RuleFor(x => x.RefundId)
            .NotEmpty()
            .WithMessage("RefundId is required.");

    }
}
