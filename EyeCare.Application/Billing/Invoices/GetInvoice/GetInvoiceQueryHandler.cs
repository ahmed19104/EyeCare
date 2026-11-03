using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Billing.Invoices.Specifications;
using EyeCare.Application.Exceptions;
using EyeCare.Domain.Entities;
using MediatR;

namespace EyeCare.Application.Billing.Invoices.GetInvoice;

public sealed class GetInvoiceQueryHandler
    : IRequestHandler<GetInvoiceQuery, GetInvoiceResult>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetInvoiceQueryHandler(
        IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<GetInvoiceResult> Handle(
        GetInvoiceQuery request,
        CancellationToken cancellationToken)
    {
        var invoice =
            await _invoiceRepository.GetForFinancialUpdateAsync(
                    request.InvoiceId,
                cancellationToken);

        if (invoice is null)
        {
            throw new NotFoundException(
                "Invoice",
                request.InvoiceId);
        }

        var items = invoice.Items
            .Select(x => new InvoiceItemResult(
                x.Id,
                x.Description,
                x.Quantity,
                x.UnitPrice,
                x.Total))
            .ToList();

        var payments = invoice.Payments
            .Select(x => new PaymentResult(
                x.Id,
                x.Amount,
                x.PaymentMethod,
                x.Status,
                x.ExternalTransactionId))
            .ToList();

        return new GetInvoiceResult(
            invoice.Id,
            invoice.AppointmentId,
            invoice.Status,
            invoice.SubTotal,
            invoice.Discount,
            invoice.Tax,
            invoice.Total,
            invoice.PaidAmount,
            invoice.RefundedAmount,
            invoice.NetPaidAmount,
            invoice.RemainingAmount,
            items,
            payments);
    }
}