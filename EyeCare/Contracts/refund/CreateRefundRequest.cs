namespace EyeCareAPI.Contracts.refund
{
    public sealed record CreateRefundRequest(
     Guid PaymentId,
     decimal Amount);
}
