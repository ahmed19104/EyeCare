using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Appointments.Comends.CheckInAppointment.Specifications;
using EyeCare.Application.Exceptions;
using EyeCare.Domain.Entities;
using MediatR;

namespace EyeCare.Application.Appointments.Comends.CheckInAppointment;

public sealed class CheckInAppointmentCommandHandler
    : IRequestHandler<CheckInAppointmentCommand>
{
    private readonly IRepository<Appointment> _appointmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CheckInAppointmentCommandHandler(
        IRepository<Appointment> appointmentRepository,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        CheckInAppointmentCommand request,
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

        appointment.CheckIn();

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}