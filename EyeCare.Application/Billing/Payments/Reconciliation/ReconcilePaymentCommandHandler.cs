using EyeCare.Application.Abstractions.Payments;
using EyeCare.Application.Abstractions.Payments.Contract;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Billing.Invoices.Specifications;
using EyeCare.Application.Billing.Payments.Specifications;
using EyeCare.Application.Exceptions;
using EyeCare.Domain.Entities;
using EyeCare.Domain.Enums;
using MediatR;

namespace EyeCare.Application.Billing.Payments.Reconciliation;

public sealed class ReconcilePaymentCommandHandler
    : IRequestHandler<
        ReconcilePaymentCommand,
        ReconcilePaymentResult>
{
    private readonly IRepository<Payment> _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IRepository<Invoice> _invoiceRepository;

    public ReconcilePaymentCommandHandler(
        IRepository<Payment> paymentRepository,
        IUnitOfWork unitOfWork,
        IPaymentGateway paymentGateway,
        IRepository<Invoice> invoiceRepository)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
        _paymentGateway = paymentGateway;
        _invoiceRepository = invoiceRepository;
    }

    public async Task<ReconcilePaymentResult> Handle(
        ReconcilePaymentCommand request,
        CancellationToken cancellationToken)
    {
        var payment =
            await _paymentRepository.FirstOrDefaultAsync(
                new PaymentForReconciliationSpecification(
                    request.PaymentId),
                cancellationToken);

        if (payment is null)
        {
            throw new NotFoundException(
                "Payment",
                request.PaymentId);
        }

        if (payment.Status != PaymentStatus.Pending)
        {
            return CreateResult(payment);
        }

        var invoice =
          await _invoiceRepository.FirstOrDefaultAsync(
              new InvoiceForPaymentSpecification(
                  payment.InvoiceId),
              cancellationToken);

        if (invoice is null)
        {
            throw new NotFoundException(
                "Invoice",
                payment.InvoiceId);
        }

        var attempt =
            payment.Attempts
                .FirstOrDefault(x =>
                    x.Status == PaymentAttemptStatus.Unknown);

        if (attempt is null)
        {
            throw new PaymentProcessingException(
                "No unknown payment attempt was found.");
        }

        PaymentGatewayStatusResult gatewayResult;

        try
        {
            gatewayResult =
                await _paymentGateway.GetPaymentStatusAsync(
                    payment.IdempotencyKey,
                    cancellationToken);
        }
        catch (OperationCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            throw new PaymentProcessingException(
                "Payment gateway status request timed out.");
        }

        switch (gatewayResult.Status)
        {
            case PaymentGatewayResultStatus.Succeeded:

                if (string.IsNullOrWhiteSpace(
                        gatewayResult.ExternalTransactionId))
                {
                    throw new PaymentProcessingException(
                        "Gateway returned success without a transaction id.");
                }

                attempt.MarkAsSucceeded(
                    gatewayResult.ExternalTransactionId);

                payment.MarkAsSucceeded(
                    gatewayResult.ExternalTransactionId);

                invoice.RefreshPaymentStatus();

                break;

            case PaymentGatewayResultStatus.Failed:

                attempt.MarkAsFailed(
                    gatewayResult.ErrorMessage
                    ?? "Payment gateway reported payment failure.");

                payment.MarkAsFailed();

                break;

            case PaymentGatewayResultStatus.Unknown:

                // We still don't know what happened.
                // Keep Payment Pending and Attempt Unknown.

                return CreateResult(payment);

            default:

                throw new PaymentProcessingException(
                    "Unsupported payment gateway status.");
        }

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return CreateResult(payment);
    }

    private static ReconcilePaymentResult CreateResult(
        Payment payment)
    {
        var attempt =
            payment.Attempts
                .OrderByDescending(x => x.CreatedAt)
                .First();

        return new ReconcilePaymentResult(
            payment.Id,
            payment.Status,
            attempt.Status,
            payment.ExternalTransactionId);
    }
}