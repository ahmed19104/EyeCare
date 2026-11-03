using MediatR;

namespace EyeCare.Application.Billing.Payments.Reconciliation;

public sealed record ReconcilePaymentCommand(
    Guid PaymentId)
    : IRequest<ReconcilePaymentResult>;
