using EyeCare.Application.Abstractions.Payments;
using EyeCare.Application.Abstractions.Persistence;

using EyeCare.Application.Billing.Payments.Specifications;
using EyeCare.Application.Exceptions;
using EyeCare.Domain.Entities;
using EyeCare.Domain.Enums;
using MediatR;

namespace EyeCare.Application.Billing.Payments.Refunds.ReconcileRefund;

public sealed class ReconcileRefundCommandHandler
    : IRequestHandler<
        ReconcileRefundCommand,
        ReconcileRefundResult>
{
    private readonly IRepository<Refund> _refundRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentGateway _paymentGateway;

    public ReconcileRefundCommandHandler(
        IRepository<Refund> refundRepository,
        IInvoiceRepository invoiceRepository,
        IUnitOfWork unitOfWork,
        IPaymentGateway paymentGateway)
    {
        _refundRepository = refundRepository;
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
        _paymentGateway = paymentGateway;
    }

    public async Task<ReconcileRefundResult> Handle(
        ReconcileRefundCommand request,
        CancellationToken cancellationToken)
    {
        var refund =
            await _refundRepository.FirstOrDefaultAsync(
                new RefundForReconciliationSpecification(
                    request.RefundId),
                cancellationToken);

        if (refund is null)
        {
            throw new NotFoundException(
                "Refund",
                request.RefundId);
        }

        // Already resolved.
        if (refund.Status != RefundStatus.Pending)
        {
            return CreateResult(refund);
        }

        var attempt = refund.Attempts
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefault();

        if (attempt is null)
        {
            throw new PaymentProcessingException(
                "Refund does not have an attempt to reconcile.");
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

        var gatewayResult =
            await _paymentGateway.GetRefundStatusAsync(
                refund.IdempotencyKey,
                cancellationToken);

        switch (gatewayResult.Status)
        {
            case PaymentGatewayResultStatus.Succeeded:

                if (string.IsNullOrWhiteSpace(
                        gatewayResult.ExternalTransactionId))
                {
                    throw new PaymentProcessingException(
                        "Refund gateway returned success without a transaction id.");
                }

                refund.MarkAsSucceeded(
                    gatewayResult.ExternalTransactionId);

                attempt.MarkAsSucceeded(
                    gatewayResult.ExternalTransactionId);

                

                invoice.RefreshPaymentStatus();

                break;

            case PaymentGatewayResultStatus.Failed:

                refund.MarkAsFailed();

                attempt.MarkAsFailed(
                    gatewayResult.ErrorMessage
                    ?? "Refund gateway rejected the refund.");

                break;

            case PaymentGatewayResultStatus.Unknown:

                // We still don't know what happened.
                // Keep Refund = Pending.
                // Keep Attempt = Unknown.

                break;

            default:

                throw new PaymentProcessingException(
                    "Unsupported refund gateway status.");
        }

        try
        {
            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }
        catch (ConcurrencyException)
        {
            throw new PaymentProcessingException(
                "Refund reconciliation result could not be saved. Reconciliation is required.");
        }

        return CreateResult(refund);
    }

    private static ReconcileRefundResult CreateResult(
        Refund refund)
    {
        var attempt = refund.Attempts
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefault();

        return new ReconcileRefundResult(
            refund.Id,
            refund.PaymentId,
            refund.Amount,
            refund.Status,
            attempt?.Status ?? RefundAttemptStatus.Started,
            refund.ExternalTransactionId);
    }
}
