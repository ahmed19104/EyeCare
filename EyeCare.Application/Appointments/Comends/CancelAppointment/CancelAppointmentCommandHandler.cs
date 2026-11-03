using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Appointments.Comends.CancelAppointment.Specifications;
using EyeCare.Application.Exceptions;
using EyeCare.Domain.Entities;

using MediatR;

namespace EyeCare.Application.Appointments.Comends.CancelAppointment
{

    public sealed class CancelAppointmentCommandHandler
        : IRequestHandler<CancelAppointmentCommand>
    {
        private readonly IRepository<Appointment> _appointmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelAppointmentCommandHandler(
            IRepository<Appointment> appointmentRepository,
            IUnitOfWork unitOfWork)
        {
            _appointmentRepository = appointmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(
            CancelAppointmentCommand request,
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

            appointment.Cancel();

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }
    }
}
