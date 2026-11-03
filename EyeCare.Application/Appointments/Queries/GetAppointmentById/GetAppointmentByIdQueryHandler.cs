using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Appointments.Queries.GetAppointmentById.Specifications;
using EyeCare.Application.Exceptions;
using EyeCare.Domain.Entities;
using MediatR;

namespace EyeCare.Application.Appointments.Queries.GetAppointmentById;

public sealed class GetAppointmentByIdQueryHandler
    : IRequestHandler<
        GetAppointmentByIdQuery,
        AppointmentResponse>
{
    private readonly IRepository<Appointment> _appointmentRepository;

    public GetAppointmentByIdQueryHandler(
        IRepository<Appointment> appointmentRepository)
    {
        _appointmentRepository =
            appointmentRepository;
    }

    public async Task<AppointmentResponse> Handle(
        GetAppointmentByIdQuery request,
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

        return new AppointmentResponse(
            appointment.Id,

            appointment.PatientId,
            $"{appointment.Patient.FirstName} {appointment.Patient.LastName}",

            appointment.DoctorId,
            $"{appointment.Doctor.FirstName} {appointment.Doctor.LastName}",

            appointment.BranchId,
            appointment.Branch.Name,

            appointment.StartAt,
            appointment.EndAt,

            appointment.Status);
    }
}
