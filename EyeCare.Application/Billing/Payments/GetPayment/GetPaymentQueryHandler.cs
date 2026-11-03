using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Billing.Payments.Specifications;
using EyeCare.Application.Exceptions;
using EyeCare.Domain.Entities;
using MediatR;

namespace EyeCare.Application.Billing.Payments.GetPayment;

public sealed class GetPaymentQueryHandler
    : IRequestHandler<
        GetPaymentQuery,
        GetPaymentResult>
{
    private readonly IRepository<Payment> _paymentRepository;
  

    public GetPaymentQueryHandler(
        IRepository<Payment> paymentRepository,
        IRepository<PaymentAttempt> attemptRepository)
    {
        _paymentRepository = paymentRepository;
        
    }

    public async Task<GetPaymentResult> Handle(
        GetPaymentQuery request,
        CancellationToken cancellationToken)
    {
        var payment =
            await _paymentRepository.FirstOrDefaultAsync(
                new PaymentDetailsSpecification(
                    request.PaymentId),
                cancellationToken);

        if (payment is null)
        {
            throw new NotFoundException(
                "Payment",
                request.PaymentId);
        }

        var attempts = payment.Attempts
     .Select(x => new PaymentAttemptResult(
         x.Id,
         x.AttemptKey,
         x.Status,
         x.ExternalTransactionId,
         x.FailureReason,
         x.CompletedAt))
     .ToList();

      

        var refundResults = payment.Refunds
            .Select(x => new RefundResult(
                x.Id,
                x.Amount,
                x.Status,
                x.IdempotencyKey,
                x.ExternalTransactionId))
            .ToList();

        return new GetPaymentResult(
            payment.Id,
            payment.InvoiceId,
            payment.Amount,
            payment.PaymentMethod,
            payment.Status,
            payment.IdempotencyKey,
            payment.ExternalTransactionId,
            attempts,
            refundResults);
    }
}