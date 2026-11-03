using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Appointments.Comends.StartAppointment.StartAppointment.Specifications;
using EyeCare.Application.Exceptions;
using EyeCare.Domain.Entities;
using MediatR;
namespace EyeCare.Application.Appointments.Comends.StartAppointment.StartAppointment
{
    public sealed class StartAppointmentCommandHandler
    : IRequestHandler<StartAppointmentCommand>
    {
        private readonly IRepository<Appointment> _appointmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public StartAppointmentCommandHandler(
            IRepository<Appointment> appointmentRepository,
            IUnitOfWork unitOfWork)
        {
            _appointmentRepository = appointmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(
            StartAppointmentCommand request,
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

            appointment.StartConsultation();

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }
    }
}
