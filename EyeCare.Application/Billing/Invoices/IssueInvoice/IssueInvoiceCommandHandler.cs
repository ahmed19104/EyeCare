using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Billing.Invoices.Specifications;
using EyeCare.Application.Exceptions;
using EyeCare.Domain.Entities;
using MediatR;

namespace EyeCare.Application.Billing.Invoices.IssueInvoice;

public sealed class IssueInvoiceCommandHandler
    : IRequestHandler<
        IssueInvoiceCommand,
        IssueInvoiceResult>
{
    private readonly IRepository<Invoice> _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public IssueInvoiceCommandHandler(
        IRepository<Invoice> invoiceRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IssueInvoiceResult> Handle(
        IssueInvoiceCommand request,
        CancellationToken cancellationToken)
    {
        var invoice =
            await _invoiceRepository.FirstOrDefaultAsync(
                new InvoiceByIdSpecification(
                    request.InvoiceId),
                cancellationToken);

        if (invoice is null)
        {
            throw new NotFoundException(
                "Invoice",
                request.InvoiceId);
        }

        invoice.Issue();

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new IssueInvoiceResult(
            invoice.Id,
            invoice.AppointmentId,
            invoice.Status,
            invoice.SubTotal,
            invoice.Discount,
            invoice.Tax,
            invoice.Total);
    }
}