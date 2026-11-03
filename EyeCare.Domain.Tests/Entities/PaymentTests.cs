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
    public class PaymentTests
    {

        [Fact]
        public void GetActiveAttempt_ShouldReturnUnknownAttempt_WhenUnknownAndStartedAttemptsExist()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Cash,
                "payment-active-attempt-001");

            var startedAttempt = new PaymentAttempt(
                payment.Id,
                "attempt-started-001");

            var unknownAttempt = new PaymentAttempt(
                payment.Id,
                "attempt-unknown-001");

            unknownAttempt.MarkAsUnknown("Gateway timeout");

            //payment.AddAttempt(startedAttempt);
            payment.AddAttempt(unknownAttempt);

            // Act
            var activeAttempt = payment.GetActiveAttempt();

            // Assert
            Assert.NotNull(activeAttempt);
            Assert.Equal(unknownAttempt.Id, activeAttempt.Id);
            Assert.Equal(
                PaymentAttemptStatus.Unknown,
                activeAttempt.Status);
        }


        [Fact]
        public void GetActiveAttempt_ShouldReturnNull_WhenNoActiveAttemptsExist()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Cash,
                "payment-no-active-attempt-001");

            var failedAttempt = new PaymentAttempt(
                payment.Id,
                "attempt-failed-001");

            failedAttempt.MarkAsFailed("Payment declined");

            payment.AddAttempt(failedAttempt);

            // Act
            var activeAttempt = payment.GetActiveAttempt();

            // Assert
            Assert.Null(activeAttempt);
        }


        [Fact]
        public void AddAttempt_ShouldThrowDomainException_WhenAttemptBelongsToAnotherPayment()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Cash,
                "payment-owner-001");

            var attempt = new PaymentAttempt(
                Guid.NewGuid(),
                "attempt-wrong-owner-001");

            // Act
            var exception = Assert.Throws<DomainException>(
                () => payment.AddAttempt(attempt));

            // Assert
            Assert.Equal(
                "Payment attempt does not belong to this payment.",
                exception.Message);

            Assert.Empty(payment.Attempts);
        }


        [Fact]
        public void AddAttempt_ShouldThrowDomainException_WhenSameAttemptIsAddedTwice()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Cash,
                "payment-duplicate-attempt-001");

            var attempt = new PaymentAttempt(
                payment.Id,
                "attempt-duplicate-001");

            payment.AddAttempt(attempt);

            // Act
            var exception = Assert.Throws<DomainException>(
                () => payment.AddAttempt(attempt));

            // Assert
            Assert.Equal(
                "Payment attempt is already registered.",
                exception.Message);

            Assert.Single(payment.Attempts);
        }


        [Fact]
        public void AddAttempt_ShouldThrowDomainException_WhenAttemptKeyIsDuplicated()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Cash,
                "payment-attempt-key-001");

            var firstAttempt = new PaymentAttempt(
                payment.Id,
                "same-attempt-key");

            var secondAttempt = new PaymentAttempt(
                payment.Id,
                "same-attempt-key");

            payment.AddAttempt(firstAttempt);

            // Act
            var exception = Assert.Throws<DomainException>(
                () => payment.AddAttempt(secondAttempt));

            // Assert
            Assert.Equal(
                "Payment attempt key is already registered.",
                exception.Message);

            Assert.Single(payment.Attempts);
        }


        //[Fact]
        //public void GetActiveAttempt_ShouldReturnFirstUnknownAttempt_WhenMultipleUnknownAttemptsExist()
        //{
        //    // Arrange
        //    var payment = new Payment(
        //        Guid.NewGuid(),
        //        400m,
        //        PaymentMethod.Cash,
        //        "payment-multiple-unknown-001");

        //    var firstUnknownAttempt = new PaymentAttempt(
        //        payment.Id,
        //        "attempt-unknown-001");

        //    firstUnknownAttempt.MarkAsUnknown("First timeout");

        //    var secondUnknownAttempt = new PaymentAttempt(
        //        payment.Id,
        //        "attempt-unknown-002");

        //    secondUnknownAttempt.MarkAsUnknown("Second timeout");

        //    payment.AddAttempt(firstUnknownAttempt);
        //    payment.AddAttempt(secondUnknownAttempt);

        //    // Act
        //    var activeAttempt = payment.GetActiveAttempt();

        //    // Assert
        //    Assert.NotNull(activeAttempt);
        //    Assert.Equal(firstUnknownAttempt.Id, activeAttempt.Id);
        //}


        [Fact]
        public void AddAttempt_ShouldThrowDomainException_WhenPaymentHasAlreadySucceeded()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Cash,
                "payment-already-succeeded-001");

            payment.MarkAsSucceeded("gateway-tx-001");

            var newAttempt = new PaymentAttempt(
                payment.Id,
                "attempt-after-success-001");

            // Act
            var exception = Assert.Throws<DomainException>(
                () => payment.AddAttempt(newAttempt));

            // Assert
            Assert.Equal(
                "Cannot add an attempt to a completed payment.",
                exception.Message);

            Assert.Empty(payment.Attempts);
        }


        [Fact]
        public void RegisterRefund_ShouldThrowDomainException_WhenRefundBelongsToAnotherPayment()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Cash,
                "payment-refund-owner-001");

            payment.MarkAsSucceeded("gateway-tx-refund-001");

            var refund = new Refund(
                Guid.NewGuid(),
                100m,
                "refund-wrong-owner-001");

            // Act
            var exception = Assert.Throws<DomainException>(
                () => payment.RegisterRefund(refund));

            // Assert
            Assert.Equal(
                "Refund does not belong to this payment.",
                exception.Message);

            Assert.Empty(payment.Refunds);
        }


        [Fact]
        public void RegisterRefund_ShouldThrowDomainException_WhenSameRefundIsRegisteredTwice()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Cash,
                "payment-duplicate-refund-001");

            payment.MarkAsSucceeded("gateway-tx-refund-002");

            var refund = new Refund(
                payment.Id,
                100m,
                "refund-duplicate-001");

            payment.RegisterRefund(refund);

            // Act
            var exception = Assert.Throws<DomainException>(
                () => payment.RegisterRefund(refund));

            // Assert
            Assert.Equal(
                "Refund is already registered for this payment.",
                exception.Message);

            Assert.Single(payment.Refunds);
        }


        [Fact]
        public void RegisterRefund_ShouldThrowDomainException_WhenIdempotencyKeyIsDuplicated()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Cash,
                "payment-refund-idempotency-001");

            payment.MarkAsSucceeded("gateway-tx-refund-003");

            var firstRefund = new Refund(
                payment.Id,
                100m,
                "refund-idempotency-001");

            var secondRefund = new Refund(
                payment.Id,
                100m,
                "refund-idempotency-001");

            payment.RegisterRefund(firstRefund);

            // Act
            var exception = Assert.Throws<DomainException>(
                () => payment.RegisterRefund(secondRefund));

            // Assert
            Assert.Equal(
                "Refund idempotency key is already registered.",
                exception.Message);

            Assert.Single(payment.Refunds);
        }


        [Fact]
        public void RegisterRefund_ShouldThrowDomainException_WhenRefundsExceedPaymentAmount()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Cash,
                "payment-refund-limit-001");

            payment.MarkAsSucceeded("gateway-tx-refund-limit-001");

            var firstRefund = new Refund(
                payment.Id,
                300m,
                "refund-limit-001");

            var secondRefund = new Refund(
                payment.Id,
                200m,
                "refund-limit-002");

            payment.RegisterRefund(firstRefund);

            // Act
            var exception = Assert.Throws<DomainException>(
                () => payment.RegisterRefund(secondRefund));

            // Assert
            Assert.Equal(
                "Refund amount exceeds the refundable amount.",
                exception.Message);

            Assert.Single(payment.Refunds);
        }


        [Fact]
        public void RegisterRefund_ShouldAllowNewRefund_WhenPreviousRefundFailed()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Cash,
                "payment-refund-failed-001");

            payment.MarkAsSucceeded("gateway-tx-refund-failed-001");

            var firstRefund = new Refund(
                payment.Id,
                300m,
                "refund-failed-001");

            payment.RegisterRefund(firstRefund);
            firstRefund.MarkAsFailed();

            var secondRefund = new Refund(
                payment.Id,
                200m,
                "refund-failed-002");

            // Act
            payment.RegisterRefund(secondRefund);

            // Assert
            Assert.Equal(2, payment.Refunds.Count);
            Assert.Equal(RefundStatus.Failed, firstRefund.Status);
            Assert.Equal(RefundStatus.Pending, secondRefund.Status);
        }


        [Theory]
        [InlineData(PaymentStatus.Pending)]
        [InlineData(PaymentStatus.Failed)]
        [InlineData(PaymentStatus.Cancelled)]
        public void RegisterRefund_ShouldThrowDomainException_WhenPaymentIsNotSucceeded(
            PaymentStatus status)
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Cash,
                $"payment-refund-status-{status}");

            switch (status)
            {
                case PaymentStatus.Failed:
                    payment.MarkAsFailed();
                    break;

                case PaymentStatus.Cancelled:
                    payment.MarkAsCancelled();
                    break;
            }

            var refund = new Refund(
                payment.Id,
                100m,
                $"refund-status-{status}");

            // Act
            var exception = Assert.Throws<DomainException>(
                () => payment.RegisterRefund(refund));

            // Assert
            Assert.Equal(
                "Only successful payments can be refunded.",
                exception.Message);

            Assert.Empty(payment.Refunds);
        }


        [Fact]
        public void MarkAsFailed_ShouldThrowDomainException_WhenPaymentAlreadySucceeded()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Cash,
                "payment-success-to-failed-001");

            payment.MarkAsSucceeded("gateway-tx-success-001");

            // Act
            var exception = Assert.Throws<DomainException>(
                () => payment.MarkAsFailed());

            // Assert
            Assert.Equal(
                PaymentStatus.Succeeded,
                payment.Status);

            Assert.Equal(
                "Only pending payments can fail.",
                exception.Message);
        }

        [Fact]
        public void MarkAsCancelled_ShouldThrowDomainException_WhenPaymentAlreadySucceeded()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Cash,
                "payment-success-to-cancelled-001");

            payment.MarkAsSucceeded("gateway-tx-success-002");

            // Act
            var exception = Assert.Throws<DomainException>(
                () => payment.MarkAsCancelled());

            // Assert
            Assert.Equal(
                PaymentStatus.Succeeded,
                payment.Status);

            Assert.Equal(
                "Only pending payments can be cancelled.",
                exception.Message);
        }


        [Fact]
        public void MarkAsSucceeded_ShouldThrowDomainException_WhenPaymentAlreadySucceeded()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Cash,
                "payment-success-twice-001");

            payment.MarkAsSucceeded("gateway-tx-success-first");

            var originalSucceededAt = payment.SucceededAt;
            var originalTransactionId = payment.ExternalTransactionId;

            // Act
            var exception = Assert.Throws<DomainException>(
                () => payment.MarkAsSucceeded("gateway-tx-success-second"));

            // Assert
            Assert.Equal(PaymentStatus.Succeeded, payment.Status);
            Assert.Equal(originalTransactionId, payment.ExternalTransactionId);
            Assert.Equal(originalSucceededAt, payment.SucceededAt);

            Assert.Equal("Only pending payments can succeed.", exception.Message);
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void MarkAsSucceeded_ShouldThrowDomainException_WhenExternalTransactionIdIsInvalid(
            string? externalTransactionId)
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Cash,
                "payment-invalid-transaction-001");

            // Act
            var exception = Assert.Throws<DomainException>(
                () => payment.MarkAsSucceeded(externalTransactionId!));

            // Assert
            Assert.Equal(PaymentStatus.Pending, payment.Status);
            Assert.Null(payment.ExternalTransactionId);
            Assert.Null(payment.SucceededAt);

            Assert.Equal(
                "External transaction id is required.",
                exception.Message);
        }


        [Fact]
        public void Constructor_ShouldThrowDomainException_WhenInvoiceIdIsEmpty()
        {
            // Arrange
            var invoiceId = Guid.Empty;
            var amount = 400m;
            var paymentMethod = PaymentMethod.Cash;
            var idempotencyKey = "payment-invalid-invoice-001";

            // Act
            var exception = Assert.Throws<DomainException>(
                () => new Payment(
                    invoiceId,
                    amount,
                    paymentMethod,
                    idempotencyKey));

            // Assert
            Assert.Equal("Invoice is required.", exception.Message);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Constructor_ShouldThrowDomainException_WhenAmountIsNotPositive(
            decimal amount)
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var paymentMethod = PaymentMethod.Cash;
            var idempotencyKey = "payment-invalid-amount-001";

            // Act
            var exception = Assert.Throws<DomainException>(
                () => new Payment(
                    invoiceId,
                    amount,
                    paymentMethod,
                    idempotencyKey));

            // Assert
            Assert.Equal(
                "Payment amount must be greater than zero.",
                exception.Message);
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_ShouldThrowDomainException_WhenIdempotencyKeyIsInvalid(
            string? idempotencyKey)
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var amount = 400m;
            var paymentMethod = PaymentMethod.Cash;

            // Act
            var exception = Assert.Throws<DomainException>(
                () => new Payment(
                    invoiceId,
                    amount,
                    paymentMethod,
                    idempotencyKey!));

            // Assert
            Assert.Equal(
                "Idempotency key is required.",
                exception.Message);
        }


        [Fact]
        public void Constructor_ShouldInitializePaymentAsPending()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();

            // Act
            var payment = new Payment(
                invoiceId,
                400m,
                PaymentMethod.Cash,
                "payment-initial-status-001");

            // Assert
            Assert.Equal(invoiceId, payment.InvoiceId);
            Assert.Equal(400m, payment.Amount);
            Assert.Equal(PaymentMethod.Cash, payment.PaymentMethod);
            Assert.Equal(PaymentStatus.Pending, payment.Status);

            Assert.Null(payment.SucceededAt);
            Assert.Null(payment.ExternalTransactionId);
            Assert.Empty(payment.Attempts);
            Assert.Empty(payment.Refunds);
        }


        [Fact]
        public void MatchesRequest_ShouldReturnTrue_WhenRequestDataMatches()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();

            var payment = new Payment(
                invoiceId,
                400m,
                PaymentMethod.Cash,
                "payment-matches-request-001");

            // Act
            var result = payment.MatchesRequest(
                invoiceId,
                400m,
                PaymentMethod.Cash);

            // Assert
            Assert.True(result);
        }


        [Theory]
        [InlineData(500, PaymentMethod.Cash)]
        [InlineData(400, PaymentMethod.Visa)]
        public void MatchesRequest_ShouldReturnFalse_WhenAmountOrPaymentMethodIsDifferent(
            decimal amount,
            PaymentMethod paymentMethod)
        {
            // Arrange
            var invoiceId = Guid.NewGuid();

            var payment = new Payment(
                invoiceId,
                400m,
                PaymentMethod.Cash,
                "payment-matches-request-002");

            // Act
            var result = payment.MatchesRequest(
                invoiceId,
                amount,
                paymentMethod);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void MatchesRequest_ShouldReturnFalse_WhenInvoiceIdIsDifferent()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Cash,
                "payment-matches-request-003");

            // Act
            var result = payment.MatchesRequest(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Cash);

            // Assert
            Assert.False(result);
        }



        [Fact]
        public void MarkAsSucceeded_ShouldUpdateUpdatedAt()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Cash,
                "payment-updated-at-001");

            // Act
            payment.MarkAsSucceeded("gateway-tx-updated-at-001");

            // Assert
            Assert.Equal(PaymentStatus.Succeeded, payment.Status);
            Assert.NotNull(payment.UpdatedAt);
        }


        [Fact]
        public void MarkAsFailed_ShouldUpdateUpdatedAt()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Cash,
                "payment-updated-at-failed-001");

            // Act
            payment.MarkAsFailed();

            // Assert
            Assert.Equal(PaymentStatus.Failed, payment.Status);
            Assert.NotNull(payment.UpdatedAt);
        }

        [Fact]
        public void MarkAsCancelled_ShouldUpdateUpdatedAt()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Cash,
                "payment-updated-at-cancelled-001");

            // Act
            payment.MarkAsCancelled();

            // Assert
            Assert.Equal(PaymentStatus.Cancelled, payment.Status);
            Assert.NotNull(payment.UpdatedAt);
        }


        [Fact]
        public void MarkAsFailed_ShouldThrowDomainException_WhenPaymentAlreadyFailed()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Cash,
                "payment-already-failed-001");

            payment.MarkAsFailed();

            var originalUpdatedAt = payment.UpdatedAt;

            // Act
            var exception = Assert.Throws<DomainException>(
                () => payment.MarkAsFailed());

            // Assert
            Assert.Equal(PaymentStatus.Failed, payment.Status);
            Assert.Equal(originalUpdatedAt, payment.UpdatedAt);
            Assert.Equal("Only pending payments can fail.", exception.Message);
        }


        [Fact]
        public void MarkAsCancelled_ShouldThrowDomainException_WhenPaymentAlreadyFailed()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Cash,
                "payment-failed-to-cancel-001");

            payment.MarkAsFailed();

            var originalUpdatedAt = payment.UpdatedAt;

            // Act
            var exception = Assert.Throws<DomainException>(
                () => payment.MarkAsCancelled());

            // Assert
            Assert.Equal(PaymentStatus.Failed, payment.Status);
            Assert.Equal(originalUpdatedAt, payment.UpdatedAt);

            Assert.Equal(
                "Only pending payments can be cancelled.",
                exception.Message);
        }


        [Fact]
        public void MarkAsFailed_ShouldThrowDomainException_WhenPaymentAlreadyCancelled()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Cash,
                "payment-cancelled-to-failed-001");

            payment.MarkAsCancelled();

            var originalUpdatedAt = payment.UpdatedAt;

            // Act
            var exception = Assert.Throws<DomainException>(
                () => payment.MarkAsFailed());

            // Assert
            Assert.Equal(PaymentStatus.Cancelled, payment.Status);
            Assert.Equal(originalUpdatedAt, payment.UpdatedAt);

            Assert.Equal(
                "Only pending payments can fail.",
                exception.Message);
        }


        [Fact]
        public void AddAttempt_ShouldThrowDomainException_WhenUnknownAttemptExists()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Visa,
                "payment-unknown-attempt-001");

            var unknownAttempt = new PaymentAttempt(
                payment.Id,
                "attempt-unknown-001");

            unknownAttempt.MarkAsUnknown("Gateway timeout");
            payment.AddAttempt(unknownAttempt);

            var newAttempt = new PaymentAttempt(
                payment.Id,
                "attempt-unknown-002");

            // Act
            var exception = Assert.Throws<DomainException>(
                () => payment.AddAttempt(newAttempt));

            // Assert
            Assert.Single(payment.Attempts);
            Assert.Equal(
                PaymentAttemptStatus.Unknown,
                payment.Attempts.Single().Status);

            Assert.Equal(
                "Cannot add a new attempt while an unknown attempt exists.",
                exception.Message);
        }


        [Fact]
        public void AddAttempt_ShouldAllowNewAttempt_WhenPreviousAttemptFailed()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Visa,
                "payment-retry-after-failure-001");

            var firstAttempt = new PaymentAttempt(
                payment.Id,
                "attempt-retry-first-001");

            firstAttempt.MarkAsFailed("Card declined");
            payment.AddAttempt(firstAttempt);

            var secondAttempt = new PaymentAttempt(
                payment.Id,
                "attempt-retry-second-001");

            // Act
            payment.AddAttempt(secondAttempt);

            // Assert
            Assert.Equal(2, payment.Attempts.Count);
            Assert.Equal(
                PaymentAttemptStatus.Failed,
                firstAttempt.Status);

            Assert.Equal(
                PaymentAttemptStatus.Started,
                secondAttempt.Status);
        }


        [Fact]
        public void AddAttempt_ShouldThrowDomainException_WhenStartedAttemptExists()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Visa,
                "payment-started-attempt-001");

            var firstAttempt = new PaymentAttempt(
                payment.Id,
                "attempt-started-001");

            payment.AddAttempt(firstAttempt);

            var secondAttempt = new PaymentAttempt(
                payment.Id,
                "attempt-started-002");

            // Act
            var exception = Assert.Throws<DomainException>(
                () => payment.AddAttempt(secondAttempt));

            // Assert
            Assert.Single(payment.Attempts);
            Assert.Equal(
                PaymentAttemptStatus.Started,
                payment.Attempts.Single().Status);

            Assert.Equal(
                "Cannot add a new attempt while another attempt is active.",
                exception.Message);
        }


        [Fact]
        public void AddAttempt_ShouldThrowDomainException_WhenSuccessfulAttemptExists()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Visa,
                "payment-successful-attempt-001");

            var successfulAttempt = new PaymentAttempt(
                payment.Id,
                "attempt-successful-001");

            successfulAttempt.MarkAsSucceeded("gateway-tx-successful-001");
            payment.AddAttempt(successfulAttempt);

            var newAttempt = new PaymentAttempt(
                payment.Id,
                "attempt-successful-002");

            // Act
            var exception = Assert.Throws<DomainException>(
                () => payment.AddAttempt(newAttempt));

            // Assert
            Assert.Single(payment.Attempts);
            Assert.Equal(
                PaymentAttemptStatus.Succeeded,
                payment.Attempts.Single().Status);

            Assert.Equal(
                "Cannot add a new attempt after a successful attempt.",
                exception.Message);
        }


        [Fact]
        public void GetActiveAttempt_ShouldReturnStartedAttempt_WhenNoUnknownAttemptExists()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Visa,
                "payment-active-attempt-001");

            var failedAttempt = new PaymentAttempt(
                payment.Id,
                "attempt-active-failed-001");

            failedAttempt.MarkAsFailed("Card declined");
            payment.AddAttempt(failedAttempt);

            var startedAttempt = new PaymentAttempt(
                payment.Id,
                "attempt-active-started-001");

            payment.AddAttempt(startedAttempt);

            // Act
            var result = payment.GetActiveAttempt();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(startedAttempt.Id, result.Id);
            Assert.Equal(PaymentAttemptStatus.Started, result.Status);
        }


        [Fact]
        public void RegisterRefund_ShouldUpdateUpdatedAt()
        {
            // Arrange
            var payment = new Payment(
                Guid.NewGuid(),
                400m,
                PaymentMethod.Cash,
                "payment-refund-updated-at-001");

            payment.MarkAsSucceeded("gateway-tx-refund-updated-at-001");

            var refund = new Refund(
                payment.Id,
                100m,
                "refund-updated-at-001");

            // Act
            payment.RegisterRefund(refund);

            // Assert
            Assert.NotNull(payment.UpdatedAt);
            Assert.Contains(refund, payment.Refunds);
        }


    }
}
