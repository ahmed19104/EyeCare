using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Billing.Invoices.Specifications;
using EyeCare.Application.Exceptions;
using EyeCare.Domain.Entities;
using MediatR;

namespace EyeCare.Application.Billing.Invoices.AddInvoiceItem;

public sealed class AddInvoiceItemCommandHandler
    : IRequestHandler<
        AddInvoiceItemCommand,
        AddInvoiceItemResult>
{
    private readonly IRepository<Invoice> _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddInvoiceItemCommandHandler(
        IRepository<Invoice> invoiceRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AddInvoiceItemResult> Handle(
        AddInvoiceItemCommand request,
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

        var item = invoice.AddItem(
            request.Description,
            request.Quantity,
            request.UnitPrice);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new AddInvoiceItemResult(
            item.Id,
            item.InvoiceId,
            item.Description,
            item.Quantity,
            item.UnitPrice,
            item.Total);
    }
}