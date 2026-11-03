using FluentValidation;

namespace EyeCare.Application.Billing.Payments.Reconciliation;

public sealed class ReconcilePaymentCommandValidator
    : AbstractValidator<ReconcilePaymentCommand>
{
    public ReconcilePaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .NotEmpty()
            .WithMessage("Payment is required.");
    }
}