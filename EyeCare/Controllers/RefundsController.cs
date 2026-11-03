using EyeCare.Application.Abstractions.Authorization;
using EyeCare.Application.Abstractions.Payments;
using EyeCare.Application.Abstractions.Payments.Contract;
using EyeCare.Application.Billing.Payments.Refunds.CreateRefund;
using EyeCare.Application.Billing.Payments.Refunds.ReconcileRefund;
using EyeCare.Application.Billing.Payments.Refunds.Webhook.ProcessRefundWebhook;
using EyeCareAPI.Contracts.refund;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace EyeCareAPI.Controllers;

[ApiController]
[Route("api/refunds")]
[Authorize]
public sealed class RefundsController : ControllerBase
{
    private readonly ISender _sender;

    public RefundsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize(Policy = Permissions.Billing.Refund)]
    public async Task<ActionResult<CreateRefundResult>> Create(
        [FromHeader(Name = "Idempotency-Key")]
        string idempotencyKey,
        [FromBody] CreateRefundRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateRefundCommand(
            request.PaymentId,
            request.Amount,
            idempotencyKey);

        var result = await _sender.Send(
            command,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{refundId}/reconcile")]
    [Authorize(Policy = Permissions.Billing.Refund)]
    public async Task<
    ActionResult<ReconcileRefundResult>>
    Reconcile(
        Guid refundId,
        CancellationToken cancellationToken)
    {
        var command = new ReconcileRefundCommand(
            refundId);

        var result =
            await _sender.Send(
                command,
                cancellationToken);

        return Ok(result);
    }

    
}
