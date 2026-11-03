namespace EyeCare.Application.Billing.Invoices.CreateInvoice;

public sealed record CreateInvoiceResult(
    Guid InvoiceId,
    Guid AppointmentId);
