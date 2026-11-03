using EyeCare.Application.Abstractions.Authorization;
using EyeCare.Application.Billing.Payments.CreatePayment;
using EyeCare.Application.Billing.Payments.GetPayment;
using EyeCare.Application.Billing.Payments.Reconciliation;
using EyeCareAPI.Contracts.Payments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EyeCareAPI.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize]
public sealed class PaymentsController : ControllerBase
{
    private readonly ISender _sender;

    public PaymentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize(Policy = Permissions.Billing.Create)]
    public async Task<ActionResult<CreatePaymentResult>> Create(
         [FromHeader(Name = "Idempotency-Key")]
        string idempotencyKey,
        [FromBody] CreatePaymentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreatePaymentCommand(
            request.InvoiceId,
            request.Amount,
            request.PaymentMethod,
            idempotencyKey);

        var result = await _sender.Send(
            command,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{paymentId:guid}")]
    [Authorize(Policy = Permissions.Billing.Read)]
    public async Task<ActionResult<GetPaymentResult>> GetById(
    Guid paymentId,
    CancellationToken cancellationToken)
    {
        var query = new GetPaymentQuery(
            paymentId);

        var result = await _sender.Send(
            query,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{paymentId:guid}/reconcile")]
    [Authorize(Policy = Permissions.Billing.Read)]
    public async Task<
    ActionResult<ReconcilePaymentResult>>
    Reconcile(
        Guid paymentId,
        CancellationToken cancellationToken)
    {
        var command =
            new ReconcilePaymentCommand(paymentId);

        var result =
            await _sender.Send(
                command,
                cancellationToken);

        return Ok(result);
    }
}
