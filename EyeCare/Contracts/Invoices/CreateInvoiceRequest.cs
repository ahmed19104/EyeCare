namespace EyeCareAPI.Contracts.Invoices
{
    

    public sealed record CreateInvoiceRequest(
        Guid AppointmentId);
}
