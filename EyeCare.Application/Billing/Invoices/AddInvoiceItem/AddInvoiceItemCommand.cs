using MediatR;

namespace EyeCare.Application.Billing.Invoices.AddInvoiceItem;

public sealed record AddInvoiceItemCommand(
    Guid InvoiceId,
    string Description,
    decimal Quantity,
    decimal UnitPrice
) : IRequest<AddInvoiceItemResult>;