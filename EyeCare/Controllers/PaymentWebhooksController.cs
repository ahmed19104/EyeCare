using System.Text;
using System.Text.Json;
using EyeCare.Application.Abstractions.Payments;
using EyeCare.Application.Abstractions.Payments.Contract;
using EyeCare.Application.Billing.Payments.Webhook;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EyeCareAPI.Controllers;

[ApiController]
[Route("api/webhooks/payments")]
public sealed class PaymentWebhooksController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IPaymentWebhookSignatureValidator _signatureValidator;

    public PaymentWebhooksController(
        ISender sender,
        IPaymentWebhookSignatureValidator signatureValidator)
    {
        _sender = sender;
        _signatureValidator = signatureValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Handle(
        CancellationToken cancellationToken)
    {
        Request.EnableBuffering();

        using var reader = new StreamReader(
            Request.Body,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            leaveOpen: true);

        var rawBody = await reader.ReadToEndAsync(
            cancellationToken);

        Request.Body.Position = 0;

        var signature =
            Request.Headers["X-Payment-Signature"]
                .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(signature))
        {
            return Unauthorized();
        }

        var isValid = _signatureValidator.IsValid(
            rawBody,
            signature);

        if (!isValid)
        {
            return Unauthorized();
        }

        var payload =
            JsonSerializer.Deserialize<PaymentWebhookPayload>(
                rawBody,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (payload is null)
        {
            return BadRequest("Invalid webhook payload.");
        }

        var command = new ProcessPaymentWebhookCommand(
            payload.EventId,
            payload.EventType,
            payload.IdempotencyKey,
            payload.ExternalTransactionId,
            payload.Amount);

        await _sender.Send(
            command,
            cancellationToken);

        return Ok();
    }
}
