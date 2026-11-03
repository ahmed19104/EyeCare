namespace EyeCareAPI.Contracts.Invoices
{
    public sealed record AddInvoiceItemRequest(
      string Description,
      decimal Quantity,
      decimal UnitPrice);
}
