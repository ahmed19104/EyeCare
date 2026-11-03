using EyeCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeCare.Application.Billing.Payments.Refunds.ReconcileRefund
{
    public sealed record ReconcileRefundResult(
     Guid RefundId,
     Guid PaymentId,
     decimal Amount,
     RefundStatus RefundStatus,
     RefundAttemptStatus AttemptStatus,
     string? ExternalTransactionId);
}
