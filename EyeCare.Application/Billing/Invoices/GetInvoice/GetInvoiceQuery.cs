using MediatR;

namespace EyeCare.Application.Billing.Invoices.GetInvoice;

public sealed record GetInvoiceQuery(
    Guid InvoiceId
) : IRequest<GetInvoiceResult>;
