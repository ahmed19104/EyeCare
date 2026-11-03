
using EyeCare.Domain.Entities;
using EyeCare.Domain.Enums;
using EyeCare.Domain.Exceptions;
using Xunit;

namespace EyeCare.Domain.Tests.Entities;

public class RefundAttemptTests
{
    private static RefundAttempt CreateAttempt()
    {
        return new RefundAttempt(
            Guid.NewGuid(),
            $"refund-attempt-{Guid.NewGuid():N}");
    }

    // Constructor

    [Fact]
    public void Constructor_ShouldThrow_WhenRefundIdIsEmpty()
    {
        var act = () => new RefundAttempt(
            Guid.Empty, "refund-attempt-001");

        Assert.Throws<DomainException>(act);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenAttemptKeyIsInvalid(
        string? attemptKey)
    {
        var act = () => new RefundAttempt(
            Guid.NewGuid(), attemptKey!);

        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Constructor_ShouldInitializeAttemptAsStarted()
    {
        var refundId = Guid.NewGuid();

        var attempt = new RefundAttempt(
            refundId, "refund-attempt-002");

        Assert.NotEqual(Guid.Empty, attempt.Id);
        Assert.Equal(refundId, attempt.RefundId);
        Assert.Equal("refund-attempt-002", attempt.AttemptKey);
        Assert.Equal(RefundAttemptStatus.Started, attempt.Status);
        Assert.Null(attempt.ExternalTransactionId);
        Assert.Null(attempt.FailureReason);
        Assert.Null(attempt.CompletedAt);
    }

    // MarkAsSucceeded

    [Fact]
    public void MarkAsSucceeded_ShouldSucceed_WhenAttemptIsStarted()
    {
        var attempt = CreateAttempt();

        attempt.MarkAsSucceeded("gateway-refund-001");

        Assert.Equal(RefundAttemptStatus.Succeeded, attempt.Status);
        Assert.Equal("gateway-refund-001", attempt.ExternalTransactionId);
        Assert.Null(attempt.FailureReason);
        Assert.NotNull(attempt.CompletedAt);
    }

    [Fact]
    public void MarkAsSucceeded_ShouldSucceed_WhenAttemptIsUnknown()
    {
        var attempt = CreateAttempt();
        attempt.MarkAsUnknown("Gateway timeout");

        attempt.MarkAsSucceeded("gateway-refund-002");

        Assert.Equal(RefundAttemptStatus.Succeeded, attempt.Status);
        Assert.Equal("gateway-refund-002", attempt.ExternalTransactionId);
        Assert.Null(attempt.FailureReason);
        Assert.NotNull(attempt.CompletedAt);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void MarkAsSucceeded_ShouldThrow_WhenTransactionIdIsInvalid(
        string? transactionId)
    {
        var attempt = CreateAttempt();

        var act = () => attempt.MarkAsSucceeded(transactionId!);

        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void MarkAsSucceeded_ShouldThrow_WhenAttemptAlreadySucceeded()
    {
        var attempt = CreateAttempt();
        attempt.MarkAsSucceeded("gateway-refund-003");

        var act = () => attempt.MarkAsSucceeded("gateway-refund-004");

        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void MarkAsSucceeded_ShouldThrow_WhenAttemptAlreadyFailed()
    {
        var attempt = CreateAttempt();
        attempt.MarkAsFailed("Refund declined");

        var act = () => attempt.MarkAsSucceeded("gateway-refund-005");

        Assert.Throws<DomainException>(act);
    }

    // MarkAsFailed

    [Fact]
    public void MarkAsFailed_ShouldSucceed_WhenAttemptIsStarted()
    {
        var attempt = CreateAttempt();

        attempt.MarkAsFailed("Refund declined");

        Assert.Equal(RefundAttemptStatus.Failed, attempt.Status);
        Assert.Equal("Refund declined", attempt.FailureReason);
        Assert.Null(attempt.ExternalTransactionId);
        Assert.NotNull(attempt.CompletedAt);
    }

    [Fact]
    public void MarkAsFailed_ShouldSucceed_WhenAttemptIsUnknown()
    {
        var attempt = CreateAttempt();
        attempt.MarkAsUnknown("Initial timeout");

        attempt.MarkAsFailed("Gateway confirmed failure");

        Assert.Equal(RefundAttemptStatus.Failed, attempt.Status);
        Assert.Equal("Gateway confirmed failure", attempt.FailureReason);
        Assert.Null(attempt.ExternalTransactionId);
        Assert.NotNull(attempt.CompletedAt);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void MarkAsFailed_ShouldThrow_WhenReasonIsInvalid(
        string? reason)
    {
        var attempt = CreateAttempt();

        var act = () => attempt.MarkAsFailed(reason!);

        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void MarkAsFailed_ShouldThrow_WhenAttemptAlreadyFailed()
    {
        var attempt = CreateAttempt();
        attempt.MarkAsFailed("First failure");

        var act = () => attempt.MarkAsFailed("Second failure");

        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void MarkAsFailed_ShouldThrow_WhenAttemptAlreadySucceeded()
    {
        var attempt = CreateAttempt();
        attempt.MarkAsSucceeded("gateway-refund-006");

        var act = () => attempt.MarkAsFailed("Refund declined");

        Assert.Throws<DomainException>(act);
    }

    // MarkAsUnknown

    [Fact]
    public void MarkAsUnknown_ShouldSetUnknown_WhenAttemptIsStarted()
    {
        var attempt = CreateAttempt();

        attempt.MarkAsUnknown("Gateway timeout");

        Assert.Equal(RefundAttemptStatus.Unknown, attempt.Status);
        Assert.Equal("Gateway timeout", attempt.FailureReason);
        Assert.Null(attempt.CompletedAt);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void MarkAsUnknown_ShouldThrow_WhenReasonIsInvalid(
        string? reason)
    {
        var attempt = CreateAttempt();

        var act = () => attempt.MarkAsUnknown(reason!);

        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void MarkAsUnknown_ShouldThrow_WhenAttemptAlreadyUnknown()
    {
        var attempt = CreateAttempt();
        attempt.MarkAsUnknown("First timeout");

        var act = () => attempt.MarkAsUnknown("Second timeout");

        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void MarkAsUnknown_ShouldThrow_WhenAttemptAlreadyFailed()
    {
        var attempt = CreateAttempt();
        attempt.MarkAsFailed("Refund declined");

        var act = () => attempt.MarkAsUnknown("Gateway timeout");

        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void MarkAsUnknown_ShouldThrow_WhenAttemptAlreadySucceeded()
    {
        var attempt = CreateAttempt();
        attempt.MarkAsSucceeded("gateway-refund-007");

        var act = () => attempt.MarkAsUnknown("Gateway timeout");

        Assert.Throws<DomainException>(act);
    }

    // UpdatedAt

    [Fact]
    public void MarkAsSucceeded_ShouldUpdateUpdatedAt()
    {
        var attempt = CreateAttempt();
        var before = DateTime.UtcNow;

        attempt.MarkAsSucceeded("gateway-refund-008");

        Assert.NotNull(attempt.UpdatedAt);
        Assert.True(attempt.UpdatedAt >= before);
    }

    [Fact]
    public void MarkAsFailed_ShouldUpdateUpdatedAt()
    {
        var attempt = CreateAttempt();
        var before = DateTime.UtcNow;

        attempt.MarkAsFailed("Refund declined");

        Assert.NotNull(attempt.UpdatedAt);
        Assert.True(attempt.UpdatedAt >= before);
    }

    [Fact]
    public void MarkAsUnknown_ShouldUpdateUpdatedAt()
    {
        var attempt = CreateAttempt();
        var before = DateTime.UtcNow;

        attempt.MarkAsUnknown("Gateway timeout");

        Assert.NotNull(attempt.UpdatedAt);
        Assert.True(attempt.UpdatedAt >= before);
    }
}
