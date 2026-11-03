using MediatR;

namespace EyeCare.Application.Billing.Invoices.CreateInvoice;

public sealed record CreateInvoiceCommand(
    Guid AppointmentId
) : IRequest<CreateInvoiceResult>;
