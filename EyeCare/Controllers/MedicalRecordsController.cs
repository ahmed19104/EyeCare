using EyeCare.Application.Abstractions.Authorization;
using EyeCare.Application.MedicalRecords.Commands.CreateMedicalRecord;
using EyeCare.Application.MedicalRecords.Commands.UpdateMedicalRecord;
using EyeCare.Application.MedicalRecords.Queries.GetMedicalRecordAuditHistory;
using EyeCare.Application.MedicalRecords.Queries.GetMedicalRecordById;
using EyeCare.Application.MedicalRecords.Queries.GetPatientMedicalHistory;
using EyeCareAPI.Contracts.MedicalRecords;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EyeCareAPI.Controllers;

[ApiController]
[Route("api/medical-records")]
public class MedicalRecordsController : ControllerBase
{
    private readonly ISender _sender;

    public MedicalRecordsController(ISender sender)
    {
        _sender = sender;
    }

    [Authorize(
     Policy = Permissions.MedicalRecords.Create)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateMedicalRecordRequest request,
        CancellationToken cancellationToken)
    {
        var command =
            new CreateMedicalRecordCommand(
                request.AppointmentId,
                request.Diagnosis,
                request.Notes,
                request.TreatmentPlan,
                request.FollowUpDate);

        var id =
            await _sender.Send(
                command,
                cancellationToken);

        return Created(
            $"/api/medical-records/{id}",
            new { id });
    }
    
    
    [Authorize(
     Policy = Permissions.MedicalRecords.Read)]
    [HttpGet("{medicalRecordId:guid}")]
    public async Task<IActionResult> GetById(
    Guid medicalRecordId,
    CancellationToken cancellationToken)
    {
        var query =
            new GetMedicalRecordByIdQuery(
                medicalRecordId);

        var result =
            await _sender.Send(
                query,
                cancellationToken);

        return Ok(result);
    }


    [Authorize(Policy = Permissions.MedicalRecords.Update)]
    [HttpPut("{medicalRecordId:guid}")]
    public async Task<IActionResult> Update(
    Guid medicalRecordId,
    [FromBody] UpdateMedicalRecordRequest request,
    CancellationToken cancellationToken)
    {
        var command =
            new UpdateMedicalRecordCommand(
                medicalRecordId,
                request.Diagnosis,
                request.Notes,
                request.TreatmentPlan,
                request.FollowUpDate);

        await _sender.Send(
            command,
            cancellationToken);

        return NoContent();
    }



    [Authorize(Policy = Permissions.MedicalRecords.Read)]
    [HttpGet("patient/{patientId:guid}")]
    public async Task<IActionResult> GetPatientHistory(
    Guid patientId,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken cancellationToken = default)
    {
        var query =
            new GetPatientMedicalHistoryQuery(
                patientId,
                page,
                pageSize);

        var result =
            await _sender.Send(
                query,
                cancellationToken);

        return Ok(result);
    }


    [Authorize(Policy = Permissions.MedicalRecords.Read)]
    [HttpGet("{medicalRecordId:guid}/audit-history")]
    public async Task<IActionResult> GetAuditHistory(
    Guid medicalRecordId,
    CancellationToken cancellationToken)
    {
        var query =
            new GetMedicalRecordAuditHistoryQuery(
                medicalRecordId);

        var result =
            await _sender.Send(
                query,
                cancellationToken);

        return Ok(result);
    }
}
