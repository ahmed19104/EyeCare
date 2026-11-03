namespace EyeCare.Domain.Enums;


public enum InvoiceStatus
{
    Draft = 1,
    Issued = 2,
    PartiallyPaid = 3,
    Paid = 4,
    Cancelled = 5,

    PartiallyRefunded = 6,
    Refunded = 7
}
