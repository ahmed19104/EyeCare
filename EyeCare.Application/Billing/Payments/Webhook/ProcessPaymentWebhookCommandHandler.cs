using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Billing.Invoices.Specifications;
using EyeCare.Application.Billing.Payments.Specifications;
using EyeCare.Application.Exceptions;
using EyeCare.Domain.Entities;
using MediatR;
using System.Threading;

namespace EyeCare.Application.Billing.Payments.Webhook;

public sealed class ProcessPaymentWebhookCommandHandler
    : IRequestHandler<ProcessPaymentWebhookCommand>
{
    private readonly IRepository<PaymentWebhookEvent>
        _webhookEventRepository;

    private readonly IRepository<Payment>
        _paymentRepository;

    private readonly IUnitOfWork
        _unitOfWork;
    private readonly IRepository<Invoice> _invoiceRepository;

    public ProcessPaymentWebhookCommandHandler(
        IRepository<PaymentWebhookEvent> webhookEventRepository,
        IRepository<Payment> paymentRepository,
        IUnitOfWork unitOfWork,
        IRepository<Invoice> invoiceRepository)
    {
        _webhookEventRepository =
            webhookEventRepository;

        _paymentRepository =
            paymentRepository;

        _unitOfWork =
            unitOfWork;
        _invoiceRepository = invoiceRepository;
    }

    public async Task Handle(
        ProcessPaymentWebhookCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Check whether this webhook was already processed.
        var existingEvent =
            await _webhookEventRepository
                .FirstOrDefaultAsync(
                    new PaymentWebhookEventByEventIdSpecification(
                        request.EventId),
                    cancellationToken);

        if (existingEvent is not null)
        {
            return;
        }

        // 2. Find the payment.
        var payment =
            await _paymentRepository
                .FirstOrDefaultAsync(
                    new PaymentForWebhookSpecification(
                        request.IdempotencyKey),
                    cancellationToken);

        if (payment is null)
        {
            throw new NotFoundException(
                "Payment",
                request.IdempotencyKey);
        }

        // 3. Validate amount.
        if (payment.Amount != request.Amount)
        {
            throw new PaymentProcessingException(
                "Webhook amount does not match payment amount.");
        }
      

        // 4. Create webhook event record.
        var webhookEvent =
            new PaymentWebhookEvent(
                request.EventId,
                request.EventType,
                request.IdempotencyKey);

        if (request.ExternalTransactionId is not null)
        {
            webhookEvent.SetExternalTransactionId(
                request.ExternalTransactionId);
        }

     

        switch (request.EventType)
        {
            case "payment.succeeded":

                ProcessSucceededPayment(
                    payment,
                    request);

                break;

            case "payment.failed":

                ProcessFailedPayment(
                    payment);

                break;

            default:

                throw new PaymentProcessingException(
                    $"Unsupported payment webhook event: {request.EventType}");
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

    private static void ProcessSucceededPayment(
        Payment payment,
        ProcessPaymentWebhookCommand request)
    {
        if (string.IsNullOrWhiteSpace(
                request.ExternalTransactionId))
        {
            throw new PaymentProcessingException(
                "Successful webhook must contain an external transaction id.");
        }

        // Already succeeded.
        if (payment.Status ==
            Domain.Enums.PaymentStatus.Succeeded)
        {
            return;
        }

        if (payment.Status !=
            Domain.Enums.PaymentStatus.Pending)
        {
            throw new PaymentProcessingException(
                $"Payment cannot transition from {payment.Status} to Succeeded.");
        }

        var attempt =
            payment.GetActiveAttempt();



        if (attempt is null)
        {
            throw new PaymentProcessingException(
                "No active payment attempt was found.");
        }

      

        attempt.MarkAsSucceeded(
            request.ExternalTransactionId);

        payment.MarkAsSucceeded(
            request.ExternalTransactionId);

        payment.Invoice.RefreshPaymentStatus();
    }

    private static void ProcessFailedPayment(
        Payment payment)
    {
        if (payment.Status ==
            Domain.Enums.PaymentStatus.Failed)
        {
            return;
        }

        if (payment.Status !=
            Domain.Enums.PaymentStatus.Pending)
        {
            throw new PaymentProcessingException(
                $"Payment cannot transition from {payment.Status} to Failed.");
        }

        var attempt =
            payment.GetActiveAttempt();

        if (attempt is null)
        {
            throw new PaymentProcessingException(
                "No active payment attempt was found.");
        }

        attempt.MarkAsFailed(
            "Payment gateway reported payment failure.");

        payment.MarkAsFailed();
    }
}