using EyeCare.Application.Abstractions.Authorization;
using EyeCare.Application.Patients.Commands.CreatePatient;
using EyeCare.Application.Patients.Commands.DeactivatePatient;
using EyeCare.Application.Patients.Commands.UpdatePatient;
using EyeCare.Application.Patients.Queries.GetPatientById;
using EyeCare.Application.Patients.Queries.SearchPatients;
using EyeCareAPI.Contracts.Patients;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EyeCareAPI.Controllers;

[ApiController]
[Route("api/patients")]
public class PatientsController : ControllerBase
{
    private readonly ISender _sender;

    public PatientsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePatientRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreatePatientCommand(
            request.FirstName,
            request.LastName,
            request.DateOfBirth,
            request.Gender,
            request.PhoneNumber,
            request.Email,
            request.NationalId);

        var patientId = await _sender.Send(
            command,
            cancellationToken);

        return Created(
            $"/api/patients/{patientId}",
            new
            {
                PatientId = patientId
            });
    }

    [HttpGet("{patientId:guid}")]
    public async Task<IActionResult> GetById(
    Guid patientId,
    CancellationToken cancellationToken)
    {
        var query = new GetPatientByIdQuery(
            patientId);

        var patient = await _sender.Send(
            query,
            cancellationToken);

        return Ok(patient);
    }

    [HttpGet]
    //[Authorize(Policy = Permissions.Billing.Read)]
    public async Task<IActionResult> Search(
    [FromQuery] string? search,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken cancellationToken = default)
    {
        var query = new SearchPatientsQuery(
            search,
            page,
            pageSize);

        var result = await _sender.Send(
            query,
            cancellationToken);

        return Ok(result);
    }

    [HttpPut("{patientId:guid}")]
    public async Task<IActionResult> Update(
    Guid patientId,
    [FromBody] UpdatePatientRequest request,
    CancellationToken cancellationToken)
    {
        var command = new UpdatePatientCommand(
            patientId,
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Email);

        await _sender.Send(
            command,
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{patientId:guid}")]
    public async Task<IActionResult> Deactivate(
    Guid patientId,
    CancellationToken cancellationToken)
    {
        var command = new DeactivatePatientCommand(patientId);

        await _sender.Send(command, cancellationToken);

        return NoContent();
    }
}
