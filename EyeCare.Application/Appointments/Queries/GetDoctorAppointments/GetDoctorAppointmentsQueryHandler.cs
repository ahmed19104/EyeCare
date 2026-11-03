using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Appointments.Comends.BookAppointment.Specifications;
using EyeCare.Application.Appointments.Queries.GetDoctorAppointments.Specifications;
using EyeCare.Application.Exceptions;
using EyeCare.Domain.Entities;
using MediatR;

namespace EyeCare.Application.Appointments.Queries.GetDoctorAppointments;

public sealed class GetDoctorAppointmentsQueryHandler
    : IRequestHandler<
        GetDoctorAppointmentsQuery,
        DoctorAppointmentsResponse>
{
    private readonly IRepository<Appointment> _appointmentRepository;
    private readonly IRepository<Doctor> _doctorRepository;

    public GetDoctorAppointmentsQueryHandler(
        IRepository<Appointment> appointmentRepository,
        IRepository<Doctor> doctorRepository)
    {
        _appointmentRepository = appointmentRepository;
        _doctorRepository = doctorRepository;
    }

    public async Task<DoctorAppointmentsResponse> Handle(
        GetDoctorAppointmentsQuery request,
        CancellationToken cancellationToken)
    {
        var doctor =
            await _doctorRepository.FirstOrDefaultAsync(
                new DoctorByIdSpecification(request.DoctorId),
                cancellationToken);

        if (doctor is null)
        {
            throw new NotFoundException(
                "Doctor",
                request.DoctorId);
        }

        var specification =
            new DoctorAppointmentsByDateSpecification(
                request.DoctorId,
                request.Date);

        var result =
            await _appointmentRepository.ListPagedAsync(
                specification,
                request.Page,
                request.PageSize,
                cancellationToken);

        var items =
            result.Items
                .Select(appointment =>
                    new DoctorAppointmentListItem(
                        appointment.Id,
                        appointment.PatientId,
                        $"{appointment.Patient.FirstName} {appointment.Patient.LastName}",
                        appointment.StartAt,
                        appointment.EndAt,
                        appointment.Status))
                .ToList();

        return new DoctorAppointmentsResponse(
            items,
            result.Page,
            result.PageSize,
            result.TotalCount,
            result.TotalPages);
    }
}
