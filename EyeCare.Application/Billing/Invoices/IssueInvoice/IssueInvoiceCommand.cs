using MediatR;

namespace EyeCare.Application.Billing.Invoices.IssueInvoice;

public sealed record IssueInvoiceCommand(
    Guid InvoiceId
) : IRequest<IssueInvoiceResult>;