using EyeCare.Domain.Enums;

namespace EyeCare.Application.Billing.Invoices.GetInvoice;

public sealed record GetInvoiceResult(
    Guid InvoiceId,
    Guid AppointmentId,
    InvoiceStatus Status,
    decimal SubTotal,
    decimal Discount,
    decimal Tax,
    decimal Total,
    decimal PaidAmount,
    decimal RefundedAmount,
    decimal NetPaidAmount,
    decimal RemainingAmount,
    IReadOnlyList<InvoiceItemResult> Items,
    IReadOnlyList<PaymentResult> Payments);

public sealed record InvoiceItemResult(
    Guid ItemId,
    string Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal Total);

public sealed record PaymentResult(
    Guid PaymentId,
    decimal Amount,
    PaymentMethod PaymentMethod,
    PaymentStatus Status,
    string? ExternalTransactionId);