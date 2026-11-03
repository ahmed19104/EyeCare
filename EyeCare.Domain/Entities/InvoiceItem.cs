using EyeCare.Domain.Common;
using EyeCare.Domain.Exceptions;

namespace EyeCare.Domain.Entities;

public class InvoiceItem : BaseEntity
{
    private InvoiceItem()
    {
    }

    public InvoiceItem(
        Guid invoiceId,
        string description,
        decimal quantity,
        decimal unitPrice)
    {
        if (invoiceId == Guid.Empty)
            throw new DomainException(
                "Invoice is required.");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException(
                "Invoice item description is required.");

        if (quantity <= 0)
            throw new DomainException(
                "Quantity must be greater than zero.");

        if (unitPrice < 0)
            throw new DomainException(
                "Unit price cannot be negative.");

        Id = Guid.NewGuid();

        InvoiceId = invoiceId;
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;

        Total = quantity * unitPrice;

        CreatedAt = DateTime.UtcNow;
    }

    public Guid InvoiceId { get; private set; }

    public string Description { get; private set; } = null!;

    public decimal Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal Total { get; private set; }

    public Invoice Invoice { get; private set; } = null!;
}
