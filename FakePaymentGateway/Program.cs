
var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Health Check
app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        Status = "Healthy",
        Service = "FakePaymentGateway"
    });
});

// Payment Request
app.MapPost("/payments/charge", async (
    ChargePaymentRequest request,
    CancellationToken cancellationToken) =>
{
    // Simulate Gateway Processing Delay
    if (request.DelayInMilliseconds > 0)
    {
        await Task.Delay(
            request.DelayInMilliseconds,
            cancellationToken);
    }

    // Simulate Payment Failure
    if (request.SimulateFailure)
    {
        return Results.Ok(new
        {
            TransactionId = (string?)null,
            Status = "Failed",
            Amount = request.Amount,
            Currency = request.Currency,
            Message = "Payment was declined."
        });
    }

    // Simulate Successful Payment
    return Results.Ok(new
    {
        TransactionId = $"TXN-{Guid.NewGuid():N}",
        Status = "Succeeded",
        Amount = request.Amount,
        Currency = request.Currency,
        Message = "Payment completed successfully."
    });
});

app.Run();

// Request Contract
public sealed record ChargePaymentRequest(
    string OrderId,
    decimal Amount,
    string Currency,
    bool SimulateFailure = false,
    int DelayInMilliseconds = 0);
