using EyeCare.Application.Abstractions.Billing;
using EyeCare.Application.Abstractions.Payments;
using EyeCare.Application.Abstractions.Payments.Contract;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Billing.Payments.Refunds.CreateRefund;
using EyeCare.Application.Billing.Payments.Specifications;
using EyeCare.Application.Exceptions;
using EyeCare.Domain.Entities;
using EyeCare.Domain.Enums;
using MediatR;

public sealed class CreateRefundCommandHandler
    : IRequestHandler<CreateRefundCommand, CreateRefundResult>
{
    private readonly IRepository<Payment> _paymentRepository;
    private readonly IRepository<Refund> _refundRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IRefundPolicy _refundPolicy;
    private readonly IInvoiceRepository _invoiceRepository;

    public CreateRefundCommandHandler(
        IRepository<Payment> paymentRepository,
        IRepository<Refund> refundRepository,
        IUnitOfWork unitOfWork,
        IInvoiceRepository invoiceRepository,
        IPaymentGateway paymentGateway,
        IRefundPolicy refundPolicy)
    {
        _paymentRepository = paymentRepository;
        _refundRepository = refundRepository;
        _unitOfWork = unitOfWork;
        _paymentGateway = paymentGateway;
        _refundPolicy = refundPolicy;
        _invoiceRepository = invoiceRepository;
    }

    public async Task<CreateRefundResult> Handle(
        CreateRefundCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Idempotency check
        var existingRefund =
            await _refundRepository.FirstOrDefaultAsync(
                new RefundByIdempotencyKeySpecification(
                    request.IdempotencyKey),
                cancellationToken);

        if (existingRefund is not null)
        {
            if (!existingRefund.MatchesRequest(
                    request.PaymentId,
                    request.Amount))
            {
                throw new IdempotencyKeyReuseException();
            }

            return CreateResult(existingRefund);
        }

        // 2. Load payment + existing refunds
        var payment =
            await _paymentRepository.FirstOrDefaultAsync(
                new PaymentForRefundSpecification(
                    request.PaymentId),
                cancellationToken);

        if (payment is null)
            throw new NotFoundException(
                "Payment",
                request.PaymentId);

        _refundPolicy.EnsureCanRefund(
    payment,
    request.Amount,
    DateTime.UtcNow);


        var invoice =
    await _invoiceRepository.GetForFinancialUpdateAsync(
        payment.InvoiceId,
        cancellationToken);

        if (invoice is null)
        {
            throw new NotFoundException(
                "Invoice",
                payment.InvoiceId);
        }



        if (string.IsNullOrWhiteSpace(
                payment.ExternalTransactionId))
        {
            throw new PaymentProcessingException(
                "Successful payment does not have an external transaction id.");
        }

        // 4. Create Refund
        var refund = new Refund(
            payment.Id,
            request.Amount,
            request.IdempotencyKey);

        // 5. Register refund in Payment aggregate
        payment.RegisterRefund(refund);

        // 6. Create first refund attempt
        var attempt = new RefundAttempt(
            refund.Id,
            $"{request.IdempotencyKey}:{Guid.NewGuid()}");

        refund.AddAttempt(attempt);

        // 7. Persist Pending Refund + Started Attempt
        await _refundRepository.AddAsync(
            refund,
            cancellationToken);

        try
        {
            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }
        catch (IdempotencyConflictException)
        {
            return await HandleConcurrentRefund(
                request,
                cancellationToken);
        }

        // 8. Call external gateway
        PaymentGatewayResult gatewayResult;

        try
        {
            gatewayResult =
                await _paymentGateway.RefundAsync(
                    new PaymentGatewayRefundRequest(
                        payment.ExternalTransactionId,
                        request.Amount,
                        request.IdempotencyKey),
                    cancellationToken);
        }
        catch (OperationCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            // Gateway may have processed the refund
            // but our request timed out.

            attempt.MarkAsUnknown(
                "Refund gateway request timed out.");

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return CreateResult(refund);
        }

        // 9. Resolve gateway result
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

                attempt.MarkAsUnknown(
                    gatewayResult.ErrorMessage
                    ?? "Refund gateway returned an unknown result.");

                // Refund remains Pending
                break;

            default:

                throw new PaymentProcessingException(
                    "Unsupported refund gateway result.");
        }

        // 10. Persist final state
        try
        {
            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }
        catch (ConcurrencyException)
        {
            throw new PaymentProcessingException(
                "The refund gateway returned a result, but the database update failed. Reconciliation is required.");
        }

        return CreateResult(refund);
    }

    private async Task<CreateRefundResult>
        HandleConcurrentRefund(
            CreateRefundCommand request,
            CancellationToken cancellationToken)
    {
        var concurrentRefund =
            await _refundRepository.FirstOrDefaultAsync(
                new RefundByIdempotencyKeySpecification(
                    request.IdempotencyKey),
                cancellationToken);

        if (concurrentRefund is null)
            throw new IdempotencyConflictException();

        if (!concurrentRefund.MatchesRequest(
                request.PaymentId,
                request.Amount))
        {
            throw new IdempotencyKeyReuseException();
        }

        return CreateResult(concurrentRefund);
    }

    private static CreateRefundResult CreateResult(
        Refund refund)
    {
        return new CreateRefundResult(
            refund.Id,
            refund.PaymentId,
            refund.Amount,
            refund.Status,
            refund.ExternalTransactionId);
    }
}