using FluentValidation;

namespace EyeCare.Application.Billing.Invoices.CreateInvoice;

public sealed class CreateInvoiceCommandValidator
    : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.AppointmentId)
            .NotEmpty()
            .WithMessage("Appointment is required.");
    }
}