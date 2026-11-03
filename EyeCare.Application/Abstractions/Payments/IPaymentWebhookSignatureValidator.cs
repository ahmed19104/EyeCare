namespace EyeCare.Application.Abstractions.Payments;

public interface IPaymentWebhookSignatureValidator
{
    bool IsValid(
        string payload,
        string signature);
}