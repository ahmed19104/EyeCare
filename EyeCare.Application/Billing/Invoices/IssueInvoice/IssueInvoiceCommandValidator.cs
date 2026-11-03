using FluentValidation;

namespace EyeCare.Application.Billing.Invoices.IssueInvoice;

public sealed class IssueInvoiceCommandValidator
    : AbstractValidator<IssueInvoiceCommand>
{
    public IssueInvoiceCommandValidator()
    {
        RuleFor(x => x.InvoiceId)
            .NotEmpty()
            .WithMessage("Invoice is required.");
    }
}