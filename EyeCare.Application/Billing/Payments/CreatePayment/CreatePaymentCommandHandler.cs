using EyeCare.Application.Abstractions.Payments;
using EyeCare.Application.Abstractions.Payments.Contract;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Billing.Invoices.Specifications;
using EyeCare.Application.Billing.Payments.Specifications;
using EyeCare.Application.Exceptions;
using EyeCare.Domain.Entities;
using MediatR;

namespace EyeCare.Application.Billing.Payments.CreatePayment;

public sealed class CreatePaymentCommandHandler
    : IRequestHandler<CreatePaymentCommand, CreatePaymentResult>
{
    private readonly IRepository<Invoice> _invoiceRepository;
    private readonly IRepository<Payment> _paymentRepository;
    
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentGateway _paymentGateway;

    public CreatePaymentCommandHandler(
        IRepository<Invoice> invoiceRepository,
        IRepository<Payment> paymentRepository,
        IRepository<PaymentAttempt> paymentAttemptRepository,
        IUnitOfWork unitOfWork,
        IPaymentGateway paymentGateway)
    {
        _invoiceRepository = invoiceRepository;
        _paymentRepository = paymentRepository;
       
        _unitOfWork = unitOfWork;
        _paymentGateway = paymentGateway;
    }

    public async Task<CreatePaymentResult> Handle(
        CreatePaymentCommand request,
        CancellationToken cancellationToken)
    {
        // -----------------------------------------
        // 1. Idempotency check
        // -----------------------------------------

        var existingPayment =
            await _paymentRepository.FirstOrDefaultAsync(
                new PaymentByIdempotencyKeySpecification(
                    request.IdempotencyKey),
                cancellationToken);

        if (existingPayment is not null)
        {
            if (!existingPayment.MatchesRequest(
                    request.InvoiceId,
                    request.Amount,
                    request.PaymentMethod))
            {
                throw new IdempotencyKeyReuseException();
            }

            return CreateResult(existingPayment);
        }

        // -----------------------------------------
        // 2. Load invoice
        // -----------------------------------------

        var invoice =
            await _invoiceRepository.FirstOrDefaultAsync(
                new InvoiceForPaymentSpecification(
                    request.InvoiceId),
                cancellationToken);

        if (invoice is null)
        {
            throw new NotFoundException(
                "Invoice",
                request.InvoiceId);
        }

        // -----------------------------------------
        // 3. Create Payment
        // -----------------------------------------

        var payment = new Payment(
            invoice.Id,
            request.Amount,
            request.PaymentMethod,
            request.IdempotencyKey);

        // -----------------------------------------
        // 4. Create PaymentAttempt
        // -----------------------------------------

        var attempt = new PaymentAttempt(
            payment.Id,
            $"{request.IdempotencyKey}:{Guid.NewGuid()}");

        // -----------------------------------------
        // 5. Register payment in Invoice
        // -----------------------------------------

        invoice.RegisterPayment(payment);

        // -----------------------------------------
        // 6. Save initial state
        // -----------------------------------------

        await _paymentRepository.AddAsync(
            payment,
            cancellationToken);

        payment.AddAttempt(attempt);

        try
        {
            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }
        catch (IdempotencyConflictException)
        {
            return await HandleConcurrentIdempotencyRequest(
                request,
                cancellationToken);
        }
        catch (ConcurrencyException)
        {
            throw;
        }

        // -----------------------------------------
        // 7. Call payment gateway
        // -----------------------------------------

        PaymentGatewayResult gatewayResult;

        try
        {
            gatewayResult =
                await _paymentGateway.ChargeAsync(
                    new PaymentGatewayRequest(
                        request.Amount,
                        "EGP",
                        request.IdempotencyKey),
                    cancellationToken);
        }
        catch (OperationCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            attempt.MarkAsUnknown(
                "Payment gateway request timed out.");

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return CreateResult(payment);
        }

        // -----------------------------------------
        // 8. Process gateway result
        // -----------------------------------------

        switch (gatewayResult.Status)
        {
            case PaymentGatewayResultStatus.Succeeded:

                payment.MarkAsSucceeded(
                    gatewayResult.ExternalTransactionId!);

                attempt.MarkAsSucceeded(
                    gatewayResult.ExternalTransactionId!);

                invoice.RefreshPaymentStatus();

                break;

            case PaymentGatewayResultStatus.Failed:

                payment.MarkAsFailed();

                attempt.MarkAsFailed(
                    gatewayResult.ErrorMessage
                    ?? "Payment gateway rejected the payment.");

                break;

            case PaymentGatewayResultStatus.Unknown:

                attempt.MarkAsUnknown(
                    gatewayResult.ErrorMessage
                    ?? "Payment gateway returned an unknown result.");

                break;

            default:

                throw new InvalidOperationException(
                    "Unsupported payment gateway result.");
        }

        // -----------------------------------------
        // 9. Save gateway result
        // -----------------------------------------

        try
        {
            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }
        catch (ConcurrencyException)
        {
            /*
             * VERY IMPORTANT:
             *
             * Gateway already returned a result.
             *
             * DO NOT call ChargeAsync again.
             *
             * Reconciliation must resolve the final state.
             */

            throw new PaymentProcessingException(
                "The payment gateway returned a result, but the final database update could not be completed. Reconciliation is required.");
        }

        return CreateResult(payment);
    }

    private async Task<CreatePaymentResult>
        HandleConcurrentIdempotencyRequest(
            CreatePaymentCommand request,
            CancellationToken cancellationToken)
    {
        var concurrentPayment =
            await _paymentRepository.FirstOrDefaultAsync(
                new PaymentByIdempotencyKeySpecification(
                    request.IdempotencyKey),
                cancellationToken);

        if (concurrentPayment is null)
        {
            throw new IdempotencyConflictException();
        }

        if (!concurrentPayment.MatchesRequest(
                request.InvoiceId,
                request.Amount,
                request.PaymentMethod))
        {
            throw new IdempotencyKeyReuseException();
        }

        return CreateResult(concurrentPayment);
    }

    private static CreatePaymentResult CreateResult(
        Payment payment)
    {
        return new CreatePaymentResult(
            payment.Id,
            payment.Status,
            payment.Amount,
            payment.ExternalTransactionId);
    }
}