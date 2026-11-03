using MediatR;

namespace EyeCare.Application.Billing.Payments.GetPayment;

public sealed record GetPaymentQuery(
    Guid PaymentId
) : IRequest<GetPaymentResult>;