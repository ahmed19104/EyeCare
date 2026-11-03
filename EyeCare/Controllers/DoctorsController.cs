using EyeCare.Application.Doctors.Commands.AssignDoctorToBranch;
using EyeCare.Application.Doctors.Commands.CreateDoctor;
using EyeCare.Application.Doctors.Commands.CreateDoctorSchedule;
using EyeCare.Application.Doctors.Queries.GetAvailableSlots;
using EyeCareAPI.Contracts.Doctors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EyeCareAPI.Controllers;

[ApiController]
[Route("api/doctors")]
public class DoctorsController : ControllerBase
{
    private readonly ISender _sender;

    public DoctorsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateDoctorRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateDoctorCommand(
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Specialty);

        var doctorId = await _sender.Send(
            command,
            cancellationToken);

        return Created(
            $"/api/doctors/{doctorId}",
            new
            {
                DoctorId = doctorId
            });
    }
    [HttpPost("{doctorId:guid}/branches/{branchId:guid}")]
    public async Task<IActionResult> AssignToBranch(
    Guid doctorId,
    Guid branchId,
    CancellationToken cancellationToken)
    {
        var command = new AssignDoctorToBranchCommand(
            doctorId,
            branchId);

        await _sender.Send(
            command,
            cancellationToken);

        return NoContent();
    }

    [HttpPost("schedule")]
    public async Task<IActionResult> CreateSchedule(
    [FromBody] CreateDoctorScheduleRequest request,
    CancellationToken cancellationToken)
    {
        var command = new CreateDoctorScheduleCommand(
            request.DoctorId,
            request.BranchId,
            request.Day,
            request.StartTime,
            request.EndTime,
            request.SlotDurationMinutes);

        var scheduleId = await _sender.Send(
            command,
            cancellationToken);

        return Created(
            $"/api/doctors/schedule/{scheduleId}",
            new
            {
                ScheduleId = scheduleId
            });
    }

    [HttpGet("{doctorId:guid}/branches/{branchId:guid}/available-slots")]
    public async Task<IActionResult> GetAvailableSlots(
    Guid doctorId,
    Guid branchId,
    [FromQuery] DateOnly date,
    CancellationToken cancellationToken)
    {
        var query = new GetAvailableSlotsQuery(
            doctorId,
            branchId,
            date);

        var slots = await _sender.Send(
            query,
            cancellationToken);

        var response = slots
            .Select(x => new AvailableSlotResponse(x))
            .ToList();

        return Ok(response);
    }
}