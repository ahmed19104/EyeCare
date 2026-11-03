using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Appointments.Comends.MarkAppointmentAsNoShow.Specifications;
using EyeCare.Application.Exceptions;
using EyeCare.Domain.Entities;
using MediatR;

namespace EyeCare.Application.Appointments.Comends.MarkAppointmentAsNoShow;

public sealed class MarkAppointmentAsNoShowCommandHandler
    : IRequestHandler<MarkAppointmentAsNoShowCommand>
{
    private readonly IRepository<Appointment> _appointmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkAppointmentAsNoShowCommandHandler(
        IRepository<Appointment> appointmentRepository,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        MarkAppointmentAsNoShowCommand request,
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

        appointment.MarkAsNoShow();

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}