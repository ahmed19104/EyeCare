using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Appointments.Comends.StartAppointment.StartAppointment.Specifications;
using EyeCare.Application.Exceptions;
using EyeCare.Domain.Entities;
using MediatR;

namespace EyeCare.Application.Billing.Invoices.CreateInvoice;

public sealed class CreateInvoiceCommandHandler
    : IRequestHandler<CreateInvoiceCommand, CreateInvoiceResult>
{
    private readonly IRepository<Appointment> _appointmentRepository;
    private readonly IRepository<Invoice> _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInvoiceCommandHandler(
        IRepository<Appointment> appointmentRepository,
        IRepository<Invoice> invoiceRepository,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateInvoiceResult> Handle(
        CreateInvoiceCommand request,
        CancellationToken cancellationToken)
    {
        var appointment =
            await _appointmentRepository.FirstOrDefaultAsync(
                new AppointmentByIdSpecification(
                    request.AppointmentId),
                cancellationToken);

        if (appointment is null)
        {
            throw new NotFoundException(
                "Appointment",
                request.AppointmentId);
        }

        var invoice = new Invoice(
            appointment.Id);

        await _invoiceRepository.AddAsync(
            invoice,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new CreateInvoiceResult(
            invoice.Id,
            invoice.AppointmentId);
    }
}