using EyeCare.Application.Abstractions.Payments;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace EyeCare.Infrastructure.Payments;

public sealed class FakePaymentWebhookSignatureValidator
    : IPaymentWebhookSignatureValidator
{
    private readonly string _secret;

    public FakePaymentWebhookSignatureValidator(
        IConfiguration configuration)
    {
        _secret =
            configuration["Payments:WebhookSecret"]
            ?? throw new InvalidOperationException(
                "Payment webhook secret is not configured.");
    }

    public bool IsValid(
        string payload,
        string signature)
    {
        var keyBytes =
            Encoding.UTF8.GetBytes(_secret);

        var payloadBytes =
            Encoding.UTF8.GetBytes(payload);

        using var hmac =
            new HMACSHA256(keyBytes);

        var hash =
            hmac.ComputeHash(payloadBytes);

        var expectedSignature =
            Convert.ToHexString(hash);

        return CryptographicOperations
            .FixedTimeEquals(
                Encoding.UTF8.GetBytes(
                    expectedSignature),
                Encoding.UTF8.GetBytes(
                    signature));
    }
}