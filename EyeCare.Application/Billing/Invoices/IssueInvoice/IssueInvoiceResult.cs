using EyeCare.Domain.Enums;

namespace EyeCare.Application.Billing.Invoices.IssueInvoice;

public sealed record IssueInvoiceResult(
    Guid InvoiceId,
    Guid AppointmentId,
    InvoiceStatus Status,
    decimal SubTotal,
    decimal Discount,
    decimal Tax,
    decimal Total);