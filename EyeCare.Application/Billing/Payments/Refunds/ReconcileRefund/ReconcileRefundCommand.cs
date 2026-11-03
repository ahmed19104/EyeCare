using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeCare.Application.Billing.Payments.Refunds.ReconcileRefund
{
    public sealed record ReconcileRefundCommand(
     Guid RefundId)
     : IRequest<ReconcileRefundResult>;
}
