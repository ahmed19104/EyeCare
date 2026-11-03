using EyeCare.Domain.Common;
using EyeCare.Domain.Enums;
using EyeCare.Domain.Exceptions;
using System;

namespace EyeCare.Domain.Entities;

public class Invoice : BaseEntity
{
    private readonly List<InvoiceItem> _items = [];
    private readonly List<Payment> _payments = [];
    public byte[] RowVersion { get; private set; } = [];


    private Invoice()
    {
    }

    public Invoice(Guid appointmentId)
    {
        if (appointmentId == Guid.Empty)
            throw new DomainException(
                "Appointment is required.");

        Id = Guid.NewGuid();

        AppointmentId = appointmentId;

        Status = InvoiceStatus.Draft;

        Discount = 0;
        Tax = 0;

        CreatedAt = DateTime.UtcNow;
    }

    public Guid AppointmentId { get; private set; }

    public InvoiceStatus Status { get; private set; }

    public decimal SubTotal =>
        _items.Sum(x => x.Total);

    public decimal Discount { get; private set; }

    public decimal Tax { get; private set; }

    public decimal Total =>
        SubTotal - Discount + Tax;

    public decimal PaidAmount =>
     _payments
         .Where(x =>
             x.Status == PaymentStatus.Succeeded)
         .Sum(x => x.Amount);

    public decimal RefundedAmount =>
        _payments
            .SelectMany(x => x.Refunds)
            .Where(x =>
                x.Status == RefundStatus.Succeeded)
            .Sum(x => x.Amount);

    public decimal NetPaidAmount =>
        PaidAmount - RefundedAmount;

    public decimal RemainingAmount =>
        Total - NetPaidAmount;


    public decimal PendingPaymentAmount => _payments
        .Where(x =>
            x.Status == PaymentStatus.Pending 
            )
        .Sum(x => x.Amount);


    public IReadOnlyCollection<InvoiceItem> Items =>
        _items.AsReadOnly();

    public IReadOnlyCollection<Payment> Payments =>
        _payments.AsReadOnly();

    public Appointment Appointment { get; private set; } = null!;

    public InvoiceItem AddItem(
        string description,
        decimal quantity,
        decimal unitPrice)
    {
        EnsureDraft();

        var item = new InvoiceItem(
            Id,
            description,
            quantity,
            unitPrice);

        _items.Add(item);
        UpdatedAt = DateTime.UtcNow;

        return item;
    }

    public void RemoveItem(Guid itemId)
    {
        EnsureDraft();

        var item =
            _items.FirstOrDefault(
                x => x.Id == itemId);

        if (item is null)
            throw new DomainException(
                "Invoice item was not found.");

        _items.Remove(item);

        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDiscount(decimal discount)
    {
        EnsureDraft();

        if (discount < 0)
            throw new DomainException(
                "Discount cannot be negative.");

        if (discount > SubTotal)
            throw new DomainException(
                "Discount cannot exceed subtotal.");

        Discount = discount;

        UpdatedAt = DateTime.UtcNow;
    }

    public void SetTax(decimal tax)
    {
        EnsureDraft();

        if (tax < 0)
            throw new DomainException(
                "Tax cannot be negative.");

        Tax = tax;

        UpdatedAt = DateTime.UtcNow;
    }

    public void Issue()
    {
        EnsureDraft();

        if (!_items.Any())
            throw new DomainException(
                "Invoice must contain at least one item.");

        if (Total <= 0)
            throw new DomainException(
                "Invoice total must be greater than zero.");

        Status = InvoiceStatus.Issued;

        UpdatedAt = DateTime.UtcNow;
    }

    public void RegisterPayment(Payment payment)
    {
        if (payment is null)
            throw new DomainException(
                "Payment is required.");
        
if (payment.InvoiceId != Id)
            throw new DomainException(
                "Payment does not belong to this invoice.");

        if (Status is
            InvoiceStatus.Draft or
            InvoiceStatus.Cancelled)
        {
            throw new DomainException(
                "Payment cannot be registered for this invoice.");
        }

        if (Status == InvoiceStatus.Paid)
        {
            throw new DomainException(
                "Invoice is already fully paid.");
        }


        var committedAmount = NetPaidAmount + PendingPaymentAmount;

        if (payment.Amount > Total - committedAmount)
            throw new DomainException(
                "Payment amount exceeds the remaining invoice amount.");


        _payments.Add(payment);

       

        UpdatedAt = DateTime.UtcNow;
    }
    public void RefreshPaymentStatus()
    {
        if (Status == InvoiceStatus.Cancelled)
            return;

        UpdatePaymentStatus();

            UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == InvoiceStatus.Cancelled)
            throw new DomainException(
                "Invoice is already cancelled.");

        if (PaidAmount > 0)
            throw new DomainException(
                "An invoice with payments cannot be cancelled.");

        Status = InvoiceStatus.Cancelled;

        UpdatedAt = DateTime.UtcNow;
    }

    private void UpdatePaymentStatus()
    {
     
        if (Status == InvoiceStatus.Cancelled)
            return;

        if (PaidAmount == 0)
        {
            Status = InvoiceStatus.Issued;
            UpdatedAt = DateTime.UtcNow;
            return;
        }

        if (RefundedAmount > 0 && NetPaidAmount == 0)
        {
            Status = InvoiceStatus.Refunded;
            UpdatedAt = DateTime.UtcNow;
            return;
        }

        if (RefundedAmount > 0)
        {
            Status = InvoiceStatus.PartiallyRefunded;
            UpdatedAt = DateTime.UtcNow;
            return;
        }

        if (NetPaidAmount < Total)
        {
            Status = InvoiceStatus.PartiallyPaid;
            UpdatedAt = DateTime.UtcNow;
            return;
        }

        Status = InvoiceStatus.Paid;
        UpdatedAt = DateTime.UtcNow;
    }
    

    private void EnsureDraft()
    {
        if (Status != InvoiceStatus.Draft)
        {
            throw new DomainException(
                "Invoice can only be modified while it is in draft status.");
        }
    }
}