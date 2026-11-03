using EyeCare.Domain.Entities;
using EyeCare.Domain.Enums;
using EyeCare.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeCare.Domain.Tests.Entities
{
    public class PaymentAttemptTests
    {
        [Fact]
        public void MarkAsSucceeded_ShouldSetSucceededStatus_WhenAttemptIsStarted()
        {
            var attempt = CreateAttempt();

            attempt.MarkAsSucceeded("gateway-tx-001");

            Assert.Equal(PaymentAttemptStatus.Succeeded, attempt.Status);
            Assert.Equal("gateway-tx-001", attempt.ExternalTransactionId);
            Assert.Null(attempt.FailureReason);
            Assert.NotNull(attempt.CompletedAt);
        }

        [Fact]
        public void MarkAsSucceeded_ShouldAllowTransitionFromUnknown()
        {
            var attempt = CreateAttempt();
            attempt.MarkAsUnknown("Gateway timeout");

            attempt.MarkAsSucceeded("gateway-tx-002");

            Assert.Equal(PaymentAttemptStatus.Succeeded, attempt.Status);
            Assert.Equal("gateway-tx-002", attempt.ExternalTransactionId);
            Assert.Null(attempt.FailureReason);
            Assert.NotNull(attempt.CompletedAt);
        }

        [Fact]
        public void MarkAsSucceeded_ShouldThrowDomainException_WhenAttemptAlreadySucceeded()
        {
            var attempt = CreateAttempt();
            attempt.MarkAsSucceeded("gateway-tx-003");

            var act = () => attempt.MarkAsSucceeded("gateway-tx-004");

            Assert.Throws<DomainException>(act);
        }

        [Fact]
        public void MarkAsSucceeded_ShouldThrowDomainException_WhenAttemptAlreadyFailed()
        {
            var attempt = CreateAttempt();
            attempt.MarkAsFailed("Payment declined");

            var act = () => attempt.MarkAsSucceeded("gateway-tx-005");

            Assert.Throws<DomainException>(act);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void MarkAsSucceeded_ShouldThrowDomainException_WhenTransactionIdIsInvalid(
            string? transactionId)
        {
            var attempt = CreateAttempt();

            var act = () => attempt.MarkAsSucceeded(transactionId!);

            Assert.Throws<DomainException>(act);
        }


        [Fact]
        public void MarkAsFailed_ShouldSetFailedStatus_WhenAttemptIsStarted()
        {
            var attempt = CreateAttempt();

            attempt.MarkAsFailed("Payment declined");

            Assert.Equal(PaymentAttemptStatus.Failed, attempt.Status);
            Assert.Equal("Payment declined", attempt.FailureReason);
            Assert.Null(attempt.ExternalTransactionId);
            Assert.NotNull(attempt.CompletedAt);
        }

        [Fact]
        public void MarkAsFailed_ShouldThrowDomainException_WhenAttemptAlreadyFailed()
        {
            var attempt = CreateAttempt();
            attempt.MarkAsFailed("First failure");

            var act = () => attempt.MarkAsFailed("Second failure");

            Assert.Throws<DomainException>(act);
        }

        [Fact]
        public void MarkAsFailed_ShouldThrowDomainException_WhenAttemptAlreadySucceeded()
        {
            var attempt = CreateAttempt();
            attempt.MarkAsSucceeded("gateway-tx-006");

            var act = () => attempt.MarkAsFailed("Payment declined");

            Assert.Throws<DomainException>(act);
        }

        [Fact]
        public void MarkAsFailed_ShouldThrowDomainException_WhenAttemptIsUnknown()
        {
            var attempt = CreateAttempt();
            attempt.MarkAsUnknown("Gateway timeout");

            var act = () => attempt.MarkAsFailed("Payment declined");

            Assert.Throws<DomainException>(act);
        }


        [Fact]
        public void MarkAsUnknown_ShouldSetUnknownStatus_WhenAttemptIsStarted()
        {
            var attempt = CreateAttempt();

            attempt.MarkAsUnknown("Gateway timeout");

            Assert.Equal(PaymentAttemptStatus.Unknown, attempt.Status);
            Assert.Equal("Gateway timeout", attempt.FailureReason);
            Assert.Null(attempt.CompletedAt);
        }

        [Fact]
        public void MarkAsUnknown_ShouldThrowDomainException_WhenAttemptAlreadyUnknown()
        {
            var attempt = CreateAttempt();
            attempt.MarkAsUnknown("First timeout");

            var act = () => attempt.MarkAsUnknown("Second timeout");

            Assert.Throws<DomainException>(act);
        }

        [Fact]
        public void MarkAsUnknown_ShouldThrowDomainException_WhenAttemptAlreadyFailed()
        {
            var attempt = CreateAttempt();
            attempt.MarkAsFailed("Payment declined");

            var act = () => attempt.MarkAsUnknown("Gateway timeout");

            Assert.Throws<DomainException>(act);
        }

        [Fact]
        public void MarkAsUnknown_ShouldThrowDomainException_WhenAttemptAlreadySucceeded()
        {
            var attempt = CreateAttempt();
            attempt.MarkAsSucceeded("gateway-tx-007");

            var act = () => attempt.MarkAsUnknown("Gateway timeout");

            Assert.Throws<DomainException>(act);
        }


        [Fact]
        public void Constructor_ShouldThrowDomainException_WhenPaymentIdIsEmpty()
        {
            var act = () => new PaymentAttempt(Guid.Empty, "attempt-008");

            Assert.Throws<DomainException>(act);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_ShouldThrowDomainException_WhenAttemptKeyIsInvalid(
            string? attemptKey)
        {
            var act = () => new PaymentAttempt(Guid.NewGuid(), attemptKey!);

            Assert.Throws<DomainException>(act);
        }

        [Fact]
        public void Constructor_ShouldInitializeAttemptAsStarted()
        {
            var paymentId = Guid.NewGuid();
            var attempt = new PaymentAttempt(paymentId, "attempt-009");

            Assert.NotEqual(Guid.Empty, attempt.Id);
            Assert.Equal(paymentId, attempt.PaymentId);
            Assert.Equal("attempt-009", attempt.AttemptKey);
            Assert.Equal(PaymentAttemptStatus.Started, attempt.Status);
            Assert.Null(attempt.ExternalTransactionId);
            Assert.Null(attempt.FailureReason);
            Assert.Null(attempt.CompletedAt);
        }


        [Fact]
        public void MarkAsSucceeded_ShouldUpdateUpdatedAt()
        {
            var attempt = CreateAttempt();
            var before = DateTime.UtcNow;

            attempt.MarkAsSucceeded("gateway-tx-010");

            Assert.NotNull(attempt.UpdatedAt);
            Assert.True(attempt.UpdatedAt >= before);
        }

        [Fact]
        public void MarkAsFailed_ShouldUpdateUpdatedAt()
        {
            var attempt = CreateAttempt();
            var before = DateTime.UtcNow;

            attempt.MarkAsFailed("Payment declined");

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


        private static PaymentAttempt CreateAttempt()
        {
            return new PaymentAttempt(
                Guid.NewGuid(),
                $"attempt-{Guid.NewGuid():N}");
        }


    }
}
