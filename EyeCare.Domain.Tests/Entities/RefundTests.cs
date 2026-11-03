
using EyeCare.Domain.Entities;
using EyeCare.Domain.Enums;
using EyeCare.Domain.Exceptions;
using Xunit;

namespace EyeCare.Domain.Tests.Entities;

public class RefundTests
{
    private static Refund CreateRefund()
    {
        return new Refund(
            Guid.NewGuid(),
            100m,
            $"refund-{Guid.NewGuid():N}");
    }

    // Constructor

    [Fact]
    public void Constructor_ShouldThrow_WhenPaymentIdIsEmpty()
    {
        var act = () => new Refund(
            Guid.Empty, 100m, "refund-001");

        Assert.Throws<DomainException>(act);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_ShouldThrow_WhenAmountIsNotPositive(
        decimal amount)
    {
        var act = () => new Refund(
            Guid.NewGuid(), amount, "refund-002");

        Assert.Throws<DomainException>(act);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenIdempotencyKeyIsInvalid(
        string? key)
    {
        var act = () => new Refund(
            Guid.NewGuid(), 100m, key!);

        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Constructor_ShouldInitializeRefundAsPending()
    {
        var paymentId = Guid.NewGuid();

        var refund = new Refund(
            paymentId, 150m, "refund-003");

        Assert.NotEqual(Guid.Empty, refund.Id);
        Assert.Equal(paymentId, refund.PaymentId);
        Assert.Equal(150m, refund.Amount);
        Assert.Equal("refund-003", refund.IdempotencyKey);
        Assert.Equal(RefundStatus.Pending, refund.Status);
        Assert.Empty(refund.Attempts);
    }

    // MarkAsSucceeded

    [Fact]
    public void MarkAsSucceeded_ShouldSetSucceededStatus_WhenPending()
    {
        var refund = CreateRefund();

        refund.MarkAsSucceeded("gateway-refund-001");

        Assert.Equal(RefundStatus.Succeeded, refund.Status);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void MarkAsSucceeded_ShouldThrow_WhenTransactionIdIsInvalid(
        string? transactionId)
    {
        var refund = CreateRefund();

        var act = () => refund.MarkAsSucceeded(transactionId!);

        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void MarkAsSucceeded_ShouldThrow_WhenAlreadySucceeded()
    {
        var refund = CreateRefund();
        refund.MarkAsSucceeded("gateway-refund-002");

        var act = () => refund.MarkAsSucceeded("gateway-refund-003");

        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void MarkAsSucceeded_ShouldThrow_WhenAlreadyFailed()
    {
        var refund = CreateRefund();
        refund.MarkAsFailed("Gateway declined");

        var act = () => refund.MarkAsSucceeded("gateway-refund-004");

        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void MarkAsSucceeded_ShouldThrow_WhenUnknown()
    {
        var refund = CreateRefund();
        refund.MarkAsUnknown("Gateway timeout");

        var act = () => refund.MarkAsSucceeded("gateway-refund-005");

        Assert.Throws<DomainException>(act);
    }

    // MarkAsFailed

    [Fact]
    public void MarkAsFailed_ShouldSetFailedStatus_WhenPending()
    {
        var refund = CreateRefund();

        refund.MarkAsFailed("Gateway declined");

        Assert.Equal(RefundStatus.Failed, refund.Status);
    }

    [Fact]
    public void MarkAsFailed_ShouldThrow_WhenAlreadyFailed()
    {
        var refund = CreateRefund();
        refund.MarkAsFailed("First failure");

        var act = () => refund.MarkAsFailed("Second failure");

        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void MarkAsFailed_ShouldThrow_WhenAlreadySucceeded()
    {
        var refund = CreateRefund();
        refund.MarkAsSucceeded("gateway-refund-006");

        var act = () => refund.MarkAsFailed("Gateway declined");

        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void MarkAsFailed_ShouldThrow_WhenUnknown()
    {
        var refund = CreateRefund();
        refund.MarkAsUnknown("Gateway timeout");

        var act = () => refund.MarkAsFailed("Gateway declined");

        Assert.Throws<DomainException>(act);
    }

    //[Theory]
    //[InlineData(null)]
    //[InlineData("")]
    //[InlineData("   ")]
    //public void MarkAsFailed_ShouldThrow_WhenReasonIsInvalid(
    //    string? reason)
    //{
    //    var refund = CreateRefund();

    //    var act = () => refund.MarkAsFailed(reason!);

    //    Assert.Throws<DomainException>(act);
    //}

    // MarkAsUnknown

    [Fact]
    public void MarkAsUnknown_ShouldSetUnknownStatus_WhenPending()
    {
        var refund = CreateRefund();

        refund.MarkAsUnknown("Gateway timeout");

        Assert.Equal(RefundStatus.Unknown, refund.Status);
    }

    [Fact]
    public void MarkAsUnknown_ShouldThrow_WhenAlreadyUnknown()
    {
        var refund = CreateRefund();
        refund.MarkAsUnknown("First timeout");

        var act = () => refund.MarkAsUnknown("Second timeout");

        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void MarkAsUnknown_ShouldThrow_WhenAlreadyFailed()
    {
        var refund = CreateRefund();
        refund.MarkAsFailed("Gateway declined");

        var act = () => refund.MarkAsUnknown("Gateway timeout");

        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void MarkAsUnknown_ShouldThrow_WhenAlreadySucceeded()
    {
        var refund = CreateRefund();
        refund.MarkAsSucceeded("gateway-refund-007");

        var act = () => refund.MarkAsUnknown("Gateway timeout");

        Assert.Throws<DomainException>(act);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void MarkAsUnknown_ShouldThrow_WhenReasonIsInvalid(
        string? reason)
    {
        var refund = CreateRefund();

        var act = () => refund.MarkAsUnknown(reason!);

        Assert.Throws<DomainException>(act);
    }

    // Cancel

    [Fact]
    public void Cancel_ShouldSetCancelledStatus_WhenPending()
    {
        var refund = CreateRefund();

        refund.Cancel();

        Assert.Equal(RefundStatus.Cancelled, refund.Status);
    }

    [Fact]
    public void Cancel_ShouldThrow_WhenAlreadyCancelled()
    {
        var refund = CreateRefund();
        refund.Cancel();

        var act = () => refund.Cancel();

        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Cancel_ShouldThrow_WhenAlreadySucceeded()
    {
        var refund = CreateRefund();
        refund.MarkAsSucceeded("gateway-refund-008");

        var act = () => refund.Cancel();

        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Cancel_ShouldThrow_WhenAlreadyFailed()
    {
        var refund = CreateRefund();
        refund.MarkAsFailed("Gateway declined");

        var act = () => refund.Cancel();

        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Cancel_ShouldThrow_WhenUnknown()
    {
        var refund = CreateRefund();
        refund.MarkAsUnknown("Gateway timeout");

        var act = () => refund.Cancel();

        Assert.Throws<DomainException>(act);
    }

    // MatchesRequest

    [Fact]
    public void MatchesRequest_ShouldReturnTrue_WhenRequestMatches()
    {
        var paymentId = Guid.NewGuid();
        var refund = new Refund(paymentId, 100m, "refund-009");

        Assert.True(refund.MatchesRequest(paymentId, 100m));
    }

    [Fact]
    public void MatchesRequest_ShouldReturnFalse_WhenPaymentIdDiffers()
    {
        var refund = CreateRefund();

        Assert.False(refund.MatchesRequest(
            Guid.NewGuid(), refund.Amount));
    }

    [Fact]
    public void MatchesRequest_ShouldReturnFalse_WhenAmountDiffers()
    {
        var refund = CreateRefund();

        Assert.False(refund.MatchesRequest(
            refund.PaymentId, refund.Amount + 1m));
    }

    // UpdatedAt

    [Fact]
    public void MarkAsSucceeded_ShouldUpdateUpdatedAt()
    {
        var refund = CreateRefund();
        var before = DateTime.UtcNow;

        refund.MarkAsSucceeded("gateway-refund-010");

        Assert.NotNull(refund.UpdatedAt);
        Assert.True(refund.UpdatedAt >= before);
    }

    [Fact]
    public void MarkAsFailed_ShouldUpdateUpdatedAt()
    {
        var refund = CreateRefund();
        var before = DateTime.UtcNow;

        refund.MarkAsFailed("Gateway declined");

        Assert.NotNull(refund.UpdatedAt);
        Assert.True(refund.UpdatedAt >= before);
    }

    [Fact]
    public void MarkAsUnknown_ShouldUpdateUpdatedAt()
    {
        var refund = CreateRefund();
        var before = DateTime.UtcNow;

        refund.MarkAsUnknown("Gateway timeout");

        Assert.NotNull(refund.UpdatedAt);
        Assert.True(refund.UpdatedAt >= before);
    }

    [Fact]
    public void Cancel_ShouldUpdateUpdatedAt()
    {
        var refund = CreateRefund();
        var before = DateTime.UtcNow;

        refund.Cancel();

        Assert.NotNull(refund.UpdatedAt);
        Assert.True(refund.UpdatedAt >= before);
    }
}
