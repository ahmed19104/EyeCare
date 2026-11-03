using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Appointments.Comends.CompleteAppointment.Specifications;
using EyeCare.Application.Exceptions;
using EyeCare.Domain.Entities;
using MediatR;

namespace EyeCare.Application.Appointments.Comends.CompleteAppointment;

public sealed class CompleteAppointmentCommandHandler
    : IRequestHandler<CompleteAppointmentCommand>
{
    private readonly IRepository<Appointment> _appointmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteAppointmentCommandHandler(
        IRepository<Appointment> appointmentRepository,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        CompleteAppointmentCommand request,
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

        appointment.Complete();

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}
