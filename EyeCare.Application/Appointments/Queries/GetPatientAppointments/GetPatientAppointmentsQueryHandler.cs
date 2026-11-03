using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Appointments.Queries.GetPatientAppointments.Specifications;
using EyeCare.Application.Exceptions;
using EyeCare.Application.Patients.Queries.GetPatientById.Specifications;
using EyeCare.Domain.Entities;
using MediatR;

namespace EyeCare.Application.Appointments.Queries.GetPatientAppointments;

public sealed class GetPatientAppointmentsQueryHandler
    : IRequestHandler<
        GetPatientAppointmentsQuery,
        PatientAppointmentsResponse>
{
    private readonly IRepository<Appointment>
        _appointmentRepository;

    private readonly IRepository<Patient>
        _patientRepository;

    public GetPatientAppointmentsQueryHandler(
        IRepository<Appointment> appointmentRepository,
        IRepository<Patient> patientRepository)
    {
        _appointmentRepository =
            appointmentRepository;

        _patientRepository =
            patientRepository;
    }

    public async Task<PatientAppointmentsResponse> Handle(
        GetPatientAppointmentsQuery request,
        CancellationToken cancellationToken)
    {
        var patient =
            await _patientRepository.FirstOrDefaultAsync(
                new PatientByIdSpecification(
                    request.PatientId),
                cancellationToken);

        if (patient is null)
        {
            throw new NotFoundException(
                "Patient",
                request.PatientId);
        }

        var specification =
            new PatientAppointmentsSpecification(
                request.PatientId);

        var result =
            await _appointmentRepository.ListPagedAsync(
                specification,
                request.Page,
                request.PageSize,
                cancellationToken);

        var items =
            result.Items
                .Select(appointment =>
                    new PatientAppointmentListItem(
                        appointment.Id,

                        appointment.DoctorId,
                        $"{appointment.Doctor.FirstName} {appointment.Doctor.LastName}",

                        appointment.BranchId,
                        appointment.Branch.Name,

                        appointment.StartAt,
                        appointment.EndAt,

                        appointment.Status))
                .ToList();

        return new PatientAppointmentsResponse(
            items,
            result.Page,
            result.PageSize,
            result.TotalCount,
            result.TotalPages);
    }
}