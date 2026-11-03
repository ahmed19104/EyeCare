using EyeCare.Application.Appointments.Comends.BookAppointment;
using EyeCare.Application.Appointments.Comends.CancelAppointment;
using EyeCare.Application.Appointments.Comends.CheckInAppointment;
using EyeCare.Application.Appointments.Comends.CompleteAppointment;
using EyeCare.Application.Appointments.Comends.MarkAppointmentAsNoShow;
using EyeCare.Application.Appointments.Comends.StartAppointment.StartAppointment;
using EyeCare.Application.Appointments.Queries.GetAppointmentById;
using EyeCare.Application.Appointments.Queries.GetDoctorAppointments;
using EyeCare.Application.Appointments.Queries.GetPatientAppointments;
using EyeCareAPI.Contracts.Appointments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EyeCareAPI.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly ISender _sender;

        public AppointmentsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> Book(
         [FromBody] BookAppointmentRequest request,
         CancellationToken cancellationToken)
        {
            var command = new BookAppointmentCommand(
                request.PatientId,
                request.DoctorId,
                request.BranchId,
                request.Date,
                request.StartTime);

            var appointmentId =
                await _sender.Send(
                    command,
                    cancellationToken);

            return Created(
                $"/api/appointments/{appointmentId}",
                new
                {
                    AppointmentId = appointmentId
                });
        }

        [HttpPost("{appointmentId:guid}/check-in")]
        public async Task<IActionResult> CheckIn(
    Guid appointmentId,
    CancellationToken cancellationToken)
        {
            var command =
                new CheckInAppointmentCommand(
                    appointmentId);

            await _sender.Send(
                command,
                cancellationToken);

            return NoContent();
        }


        [HttpPost("{appointmentId:guid}/start")]
        public async Task<IActionResult> Start(
    Guid appointmentId,
    CancellationToken cancellationToken)
        {
            var command =
                new StartAppointmentCommand(
                    appointmentId);

            await _sender.Send(
                command,
                cancellationToken);

            return NoContent();
        }
        [HttpPost("{appointmentId:guid}/complete")]
        public async Task<IActionResult> Complete(
     Guid appointmentId,
     CancellationToken cancellationToken)
        {
            var command =
                new CompleteAppointmentCommand(
                    appointmentId);

            await _sender.Send(
                command,
                cancellationToken);

            return NoContent();
        }

        [HttpPost("{appointmentId:guid}/cancel")]
        public async Task<IActionResult> Cancel(
    Guid appointmentId,
    CancellationToken cancellationToken)
        {
            var command =
                new CancelAppointmentCommand(
                    appointmentId);

            await _sender.Send(
                command,
                cancellationToken);

            return NoContent();
        }

        [HttpPost("{appointmentId:guid}/no-show")]
        public async Task<IActionResult> MarkAsNoShow(
    Guid appointmentId,
    CancellationToken cancellationToken)
        {
            var command =
                new MarkAppointmentAsNoShowCommand(
                    appointmentId);

            await _sender.Send(
                command,
                cancellationToken);

            return NoContent();
        }

        [HttpGet("{appointmentId:guid}")]
        public async Task<IActionResult> GetById(
    Guid appointmentId,
    CancellationToken cancellationToken)
        {
            var query =
                new GetAppointmentByIdQuery(
                    appointmentId);

            var appointment =
                await _sender.Send(
                    query,
                    cancellationToken);

            return Ok(appointment);
        }

        [HttpGet("doctor/{doctorId:guid}")]
        public async Task<IActionResult> GetDoctorAppointments(
    Guid doctorId,
    [FromQuery] DateOnly date,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken cancellationToken = default)
        {
            var query =
                new GetDoctorAppointmentsQuery(
                    doctorId,
                    date,
                    page,
                    pageSize);

            var result =
                await _sender.Send(
                    query,
                    cancellationToken);

            return Ok(result);
        }
        [HttpGet("patient/{patientId:guid}")]
        public async Task<IActionResult> GetPatientAppointments(
    Guid patientId,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken cancellationToken = default)
        {
            var query =
                new GetPatientAppointmentsQuery(
                    patientId,
                    page,
                    pageSize);

            var result =
                await _sender.Send(
                    query,
                    cancellationToken);

            return Ok(result);
        }
    }
}