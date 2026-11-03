using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Billing.Payments.Specifications;
using EyeCare.Application.Exceptions;
using EyeCare.Domain.Entities;
using EyeCare.Domain.Enums;
using MediatR;

namespace EyeCare.Application.Billing.Payments.Refunds.Webhook.ProcessRefundWebhook;

public sealed class ProcessRefundWebhookCommandHandler
    : IRequestHandler<ProcessRefundWebhookCommand>
{
    private readonly IRepository<RefundWebhookEvent>
        _webhookEventRepository;

    private readonly IRepository<Refund>
        _refundRepository;

    private readonly IInvoiceRepository
        _invoiceRepository;

    private readonly IUnitOfWork
        _unitOfWork;

    public ProcessRefundWebhookCommandHandler(
        IRepository<RefundWebhookEvent> webhookEventRepository,
        IRepository<Refund> refundRepository,
        IInvoiceRepository invoiceRepository,
        IUnitOfWork unitOfWork)
    {
        _webhookEventRepository =
            webhookEventRepository;

        _refundRepository =
            refundRepository;

        _invoiceRepository =
            invoiceRepository;

        _unitOfWork =
            unitOfWork;
    }

    public async Task Handle(
        ProcessRefundWebhookCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Duplicate webhook?
        var existingEvent =
            await _webhookEventRepository
                .FirstOrDefaultAsync(
                    new RefundWebhookEventByEventIdSpecification(
                        request.EventId),
                    cancellationToken);

        if (existingEvent is not null)
        {
            return;
        }

        // 2. Find refund.
        var refund =
            await _refundRepository
                .FirstOrDefaultAsync(
                    new RefundForWebhookSpecification(
                        request.IdempotencyKey),
                    cancellationToken);

        if (refund is null)
        {
            throw new NotFoundException(
                "Refund",
                request.IdempotencyKey);
        }

        // 3. Validate amount.
        if (refund.Amount != request.Amount)
        {
            throw new PaymentProcessingException(
                "Webhook amount does not match refund amount.");
        }

        var invoice =
          await _invoiceRepository
              .GetForFinancialUpdateAsync(
                  refund.Payment.InvoiceId,
                  cancellationToken);

        if (invoice is null)
        {
            throw new NotFoundException(
                "Invoice",
                refund.Payment.InvoiceId);
        }

        // 4. Create event.
        var webhookEvent =
            new RefundWebhookEvent(
                request.EventId,
                request.EventType,
                request.IdempotencyKey);

        if (request.ExternalTransactionId is not null)
        {
            webhookEvent.SetExternalTransactionId(
                request.ExternalTransactionId);
        }

        // 5. Process event.
        switch (request.EventType)
        {
            case "refund.succeeded":

                await ProcessSucceededRefund(
                    refund,
                    request,
                    invoice,
                    cancellationToken);

                break;

            case "refund.failed":

                ProcessFailedRefund(refund);

                break;

            default:

                throw new PaymentProcessingException(
                    $"Unsupported refund webhook event: {request.EventType}");
        }

        webhookEvent.MarkAsProcessed();

        await _webhookEventRepository.AddAsync(
            webhookEvent,
            cancellationToken);

        try
        {
            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }
        catch (IdempotencyConflictException)
        {
            // Another request processed the same webhook.
            return;
        }
    }

    private async Task ProcessSucceededRefund(
        Refund refund,
        ProcessRefundWebhookCommand request,
        Invoice invoice,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(
                request.ExternalTransactionId))
        {
            throw new PaymentProcessingException(
                "Successful refund webhook must contain an external transaction id.");
        }

        // Already succeeded.
        if (refund.Status == RefundStatus.Succeeded)
        {
          await Task.CompletedTask;
          return;
        }

        if (refund.Status != RefundStatus.Pending)
        {
            throw new PaymentProcessingException(
                $"Refund cannot transition from {refund.Status} to Succeeded.");
        }

        var attempt =
           refund.GetActiveAttempt();



        if (attempt is null)
        {
            throw new PaymentProcessingException(
                "No active Refund attempt was found.");
        }



        attempt.MarkAsSucceeded(
            request.ExternalTransactionId);

        refund.MarkAsSucceeded(
            request.ExternalTransactionId);

      

        invoice.RefreshPaymentStatus();
    }

    private static void ProcessFailedRefund(
        Refund refund)
    {
        // Already failed.
        if (refund.Status == RefundStatus.Failed)
        {
            return;
        }

        if (refund.Status != RefundStatus.Pending)
        {
            throw new PaymentProcessingException(
                $"Refund cannot transition from {refund.Status} to Failed.");
        }
        var attempt =
         refund.GetActiveAttempt();

        if (attempt is null)
        {
            throw new PaymentProcessingException(
                "No active Refund attempt was found.");
        }

        attempt.MarkAsFailed(
            "Payment gateway reported payment failure.");

        refund.MarkAsFailed();
    }
}