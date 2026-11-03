namespace EyeCare.Application.Billing.Invoices.AddInvoiceItem
{
    public sealed record AddInvoiceItemResult(
     Guid ItemId,
     Guid InvoiceId,
     string Description,
     decimal Quantity,
     decimal UnitPrice,
     decimal Total);
}
