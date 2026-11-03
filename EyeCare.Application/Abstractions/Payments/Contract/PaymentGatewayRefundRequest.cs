using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeCare.Application.Abstractions.Payments.Contract
{
    public sealed record PaymentGatewayRefundRequest(
     string PaymentExternalTransactionId,
     decimal Amount,
     string IdempotencyKey);
}
