using EyeCare.Application.Abstractions.Authorization;
using EyeCare.Application.Billing.Invoices.AddInvoiceItem;
using EyeCare.Application.Billing.Invoices.CreateInvoice;
using EyeCare.Application.Billing.Invoices.GetInvoice;
using EyeCare.Application.Billing.Invoices.IssueInvoice;
using EyeCareAPI.Contracts.Invoices;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EyeCareAPI.Controllers;

[ApiController]
[Route("api/invoices")]
[Authorize]
public sealed class InvoicesController : ControllerBase
{
    private readonly ISender _sender;

    public InvoicesController(ISender sender)
    {
        _sender = sender;
    }
    [HttpPost]
    [Authorize(Policy = Permissions.Billing.Create)]
    public async Task<ActionResult<CreateInvoiceResult>> Create(
        [FromBody] CreateInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateInvoiceCommand(
            request.AppointmentId);

        var result = await _sender.Send(
            command,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{invoiceId:guid}/items")]
    [Authorize(Policy = Permissions.Billing.Create)]
    public async Task<ActionResult<AddInvoiceItemResult>> AddItem(
    Guid invoiceId,
    [FromBody] AddInvoiceItemRequest request,
    CancellationToken cancellationToken)
    {
        var command = new AddInvoiceItemCommand(
            invoiceId,
            request.Description,
            request.Quantity,
            request.UnitPrice);

        var result = await _sender.Send(
            command,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{invoiceId:guid}/issue")]
    [Authorize(Policy = Permissions.Billing.Create)]
    public async Task<ActionResult<IssueInvoiceResult>> Issue(
    Guid invoiceId,
    CancellationToken cancellationToken)
    {
        var command = new IssueInvoiceCommand(
            invoiceId);

        var result = await _sender.Send(
            command,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{invoiceId:guid}")]
    [Authorize(Policy = Permissions.Billing.Read)]
    public async Task<ActionResult<GetInvoiceResult>> GetById(
    Guid invoiceId,
    CancellationToken cancellationToken)
    {
        var query = new GetInvoiceQuery(
            invoiceId);

        var result = await _sender.Send(
            query,
            cancellationToken);

        return Ok(result);
    }
}