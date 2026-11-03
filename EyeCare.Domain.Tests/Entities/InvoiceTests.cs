
using EyeCare.Domain.Entities;
using EyeCare.Domain.Enums;
using EyeCare.Domain.Exceptions;

namespace EyeCare.Domain.Tests.Entities;

public class InvoiceTests
{
    private static Invoice CreateInvoice()
    {
        return new Invoice(Guid.NewGuid());
    }

    private static Invoice CreateInvoiceWithItems()
    {
        var invoice = CreateInvoice();

        invoice.AddItem("Eye examination", 1, 200m);
        invoice.AddItem("Medical tests", 2, 150m);

        return invoice;
    }

    private static Invoice CreateIssuedInvoice()
    {
        var invoice = CreateInvoiceWithItems();
        invoice.Issue();

        return invoice;
    }

    [Fact]
    public void Constructor_ShouldCreateDraftInvoice_WhenAppointmentIdIsValid()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var beforeCreation = DateTime.UtcNow;

        // Act
        var invoice = new Invoice(appointmentId);

        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.NotEqual(Guid.Empty, invoice.Id);
        Assert.Equal(appointmentId, invoice.AppointmentId);
        Assert.Equal(InvoiceStatus.Draft, invoice.Status);
        Assert.Equal(0m, invoice.Discount);
        Assert.Equal(0m, invoice.Tax);
        Assert.Equal(0m, invoice.SubTotal);
        Assert.Equal(0m, invoice.Total);
        Assert.Empty(invoice.Items);
        Assert.Empty(invoice.Payments);
        Assert.InRange(invoice.CreatedAt, beforeCreation, afterCreation);
    }

    [Fact]
    public void Constructor_ShouldGenerateUniqueIds_WhenCreatingMultipleInvoices()
    {
        // Arrange & Act
        var firstInvoice = CreateInvoice();
        var secondInvoice = CreateInvoice();

        // Assert
        Assert.NotEqual(firstInvoice.Id, secondInvoice.Id);
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenAppointmentIdIsEmpty()
    {
        // Act
        var exception = Assert.Throws<DomainException>(
            () => new Invoice(Guid.Empty));

        // Assert
        Assert.Equal("Appointment is required.", exception.Message);
    }

    [Fact]
    public void AddItem_ShouldAddItemAndUpdateTotals_WhenDataIsValid()
    {
        // Arrange
        var invoice = CreateInvoice();
        var beforeUpdate = DateTime.UtcNow;

        // Act
        var item = invoice.AddItem(
            "Eye examination",
            2,
            150m);

        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.Single(invoice.Items);
        Assert.Contains(item, invoice.Items);

        Assert.Equal(invoice.Id, item.InvoiceId);
        Assert.Equal("Eye examination", item.Description);
        Assert.Equal(2m, item.Quantity);
        Assert.Equal(150m, item.UnitPrice);
        Assert.Equal(300m, item.Total);

        Assert.Equal(300m, invoice.SubTotal);
        Assert.Equal(300m, invoice.Total);
        Assert.NotNull(invoice.UpdatedAt);
        Assert.InRange(invoice.UpdatedAt.Value, beforeUpdate, afterUpdate);
    }

    [Fact]
    public void AddItem_ShouldCalculateSubtotalCorrectly_WhenAddingMultipleItems()
    {
        // Arrange
        var invoice = CreateInvoice();

        // Act
        invoice.AddItem("Examination", 1, 200m);
        invoice.AddItem("Medical tests", 2, 150m);
        invoice.AddItem("Eye drops", 3, 50m);

        // Assert
        // 200 + (2 * 150) + (3 * 50) = 650
        Assert.Equal(650m, invoice.SubTotal);
        Assert.Equal(650m, invoice.Total);
        Assert.Equal(3, invoice.Items.Count);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AddItem_ShouldThrowDomainException_WhenDescriptionIsInvalid(
        string? description)
    {
        // Arrange
        var invoice = CreateInvoice();

        // Act
        var exception = Assert.Throws<DomainException>(
            () => invoice.AddItem(description!, 1, 100m));

        // Assert
        Assert.Equal(
            "Invoice item description is required.",
            exception.Message);

        Assert.Empty(invoice.Items);
        Assert.Equal(0m, invoice.SubTotal);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddItem_ShouldThrowDomainException_WhenQuantityIsNotPositive(
        decimal quantity)
    {
        // Arrange
        var invoice = CreateInvoice();

        // Act
        var exception = Assert.Throws<DomainException>(
            () => invoice.AddItem("Examination", quantity, 100m));

        // Assert
        Assert.Equal(
            "Quantity must be greater than zero.",
            exception.Message);

        Assert.Empty(invoice.Items);
        Assert.Equal(0m, invoice.SubTotal);
    }

    [Fact]
    public void AddItem_ShouldThrowDomainException_WhenUnitPriceIsNegative()
    {
        // Arrange
        var invoice = CreateInvoice();

        // Act
        var exception = Assert.Throws<DomainException>(
            () => invoice.AddItem("Examination", 1, -10m));

        // Assert
        Assert.Equal(
            "Unit price cannot be negative.",
            exception.Message);

        Assert.Empty(invoice.Items);
        Assert.Equal(0m, invoice.SubTotal);
    }

    [Fact]
    public void RemoveItem_ShouldRemoveItemAndRecalculateSubtotal()
    {
        // Arrange
        var invoice = CreateInvoiceWithItems();
        var itemToRemove = invoice.Items.First();

        // Current subtotal: 200 + (2 * 150) = 500
        Assert.Equal(500m, invoice.SubTotal);

        // Act
        invoice.RemoveItem(itemToRemove.Id);

        // Assert
        Assert.Equal(1, invoice.Items.Count);
        Assert.DoesNotContain(
            invoice.Items,
            item => item.Id == itemToRemove.Id);

        Assert.Equal(300m, invoice.SubTotal);
        Assert.Equal(300m, invoice.Total);
    }

    [Fact]
    public void RemoveItem_ShouldThrowDomainException_WhenItemDoesNotExist()
    {
        // Arrange
        var invoice = CreateInvoiceWithItems();
        var originalCount = invoice.Items.Count;
        var originalSubtotal = invoice.SubTotal;
        var originalUpdatedAt = invoice.UpdatedAt;

        // Act
        var exception = Assert.Throws<DomainException>(
            () => invoice.RemoveItem(Guid.NewGuid()));

        // Assert
        Assert.Equal(
            "Invoice item was not found.",
            exception.Message);

        Assert.Equal(originalCount, invoice.Items.Count);
        Assert.Equal(originalSubtotal, invoice.SubTotal);
        Assert.Equal(originalUpdatedAt, invoice.UpdatedAt);
    }

    [Fact]
    public void SetDiscount_ShouldUpdateDiscountAndTotal_WhenDiscountIsValid()
    {
        // Arrange
        var invoice = CreateInvoiceWithItems();

        // Subtotal = 500
        var beforeUpdate = DateTime.UtcNow;

        // Act
        invoice.SetDiscount(100m);

        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.Equal(100m, invoice.Discount);
        Assert.Equal(400m, invoice.Total);
        Assert.NotNull(invoice.UpdatedAt);
        Assert.InRange(invoice.UpdatedAt.Value, beforeUpdate, afterUpdate);
    }

    [Fact]
    public void SetDiscount_ShouldAllowZeroDiscount()
    {
        // Arrange
        var invoice = CreateInvoiceWithItems();

        // Act
        invoice.SetDiscount(0m);

        // Assert
        Assert.Equal(0m, invoice.Discount);
        Assert.Equal(500m, invoice.Total);
    }

    [Fact]
    public void SetDiscount_ShouldThrowDomainException_WhenDiscountIsNegative()
    {
        // Arrange
        var invoice = CreateInvoiceWithItems();
        var originalDiscount = invoice.Discount;
        var originalUpdatedAt = invoice.UpdatedAt;

        // Act
        var exception = Assert.Throws<DomainException>(
            () => invoice.SetDiscount(-1m));

        // Assert
        Assert.Equal(
            "Discount cannot be negative.",
            exception.Message);

        Assert.Equal(originalDiscount, invoice.Discount);
        Assert.Equal(500m, invoice.Total);
        Assert.Equal(originalUpdatedAt, invoice.UpdatedAt);
    }

    [Fact]
    public void SetDiscount_ShouldThrowDomainException_WhenDiscountExceedsSubtotal()
    {
        // Arrange
        var invoice = CreateInvoiceWithItems();
        var originalUpdatedAt = invoice.UpdatedAt;

        // Act
        var exception = Assert.Throws<DomainException>(
            () => invoice.SetDiscount(501m));

        // Assert
        Assert.Equal(
            "Discount cannot exceed subtotal.",
            exception.Message);

        Assert.Equal(0m, invoice.Discount);
        Assert.Equal(500m, invoice.SubTotal);
        Assert.Equal(500m, invoice.Total);
        Assert.Equal(originalUpdatedAt, invoice.UpdatedAt);
    }

    [Fact]
    public void SetDiscount_ShouldAllowDiscountEqualToSubtotal()
    {
        // Arrange
        var invoice = CreateInvoiceWithItems();

        // Act
        invoice.SetDiscount(500m);

        // Assert
        Assert.Equal(500m, invoice.Discount);
        Assert.Equal(0m, invoice.Total);
    }

    [Fact]
    public void SetTax_ShouldUpdateTaxAndTotal_WhenTaxIsValid()
    {
        // Arrange
        var invoice = CreateInvoiceWithItems();

        // Act
        invoice.SetTax(50m);

        // Assert
        Assert.Equal(50m, invoice.Tax);
        Assert.Equal(550m, invoice.Total);
    }

    [Fact]
    public void SetTax_ShouldAllowZeroTax()
    {
        // Arrange
        var invoice = CreateInvoiceWithItems();

        // Act
        invoice.SetTax(0m);

        // Assert
        Assert.Equal(0m, invoice.Tax);
        Assert.Equal(500m, invoice.Total);
    }

    [Fact]
    public void SetTax_ShouldThrowDomainException_WhenTaxIsNegative()
    {
        // Arrange
        var invoice = CreateInvoiceWithItems();
        var originalTax = invoice.Tax;
        var originalUpdatedAt = invoice.UpdatedAt;

        // Act
        var exception = Assert.Throws<DomainException>(
            () => invoice.SetTax(-1m));

        // Assert
        Assert.Equal(
            "Tax cannot be negative.",
            exception.Message);

        Assert.Equal(originalTax, invoice.Tax);
        Assert.Equal(500m, invoice.Total);
        Assert.Equal(originalUpdatedAt, invoice.UpdatedAt);
    }

    [Fact]
    public void Total_ShouldCalculateSubtotalMinusDiscountPlusTax()
    {
        // Arrange
        var invoice = CreateInvoiceWithItems();

        // Subtotal = 500
        invoice.SetDiscount(100m);
        invoice.SetTax(40m);

        // Act
        var total = invoice.Total;

        // Assert
        // 500 - 100 + 40 = 440
        Assert.Equal(500m, invoice.SubTotal);
        Assert.Equal(440m, total);
    }

    [Fact]
    public void AddItem_ShouldThrowDomainException_WhenInvoiceIsNotDraft()
    {
        // Arrange
        var invoice = CreateIssuedInvoice();
        var originalCount = invoice.Items.Count;
        var originalSubtotal = invoice.SubTotal;

        // Act
        var exception = Assert.Throws<DomainException>(
            () => invoice.AddItem("Extra service", 1, 100m));

        // Assert
        Assert.Equal(
            "Invoice can only be modified while it is in draft status.",
            exception.Message);

        Assert.Equal(originalCount, invoice.Items.Count);
        Assert.Equal(originalSubtotal, invoice.SubTotal);
    }

    [Fact]
    public void RemoveItem_ShouldThrowDomainException_WhenInvoiceIsNotDraft()
    {
        // Arrange
        var invoice = CreateIssuedInvoice();
        var existingItemId = invoice.Items.First().Id;
        var originalCount = invoice.Items.Count;

        // Act
        var exception = Assert.Throws<DomainException>(
            () => invoice.RemoveItem(existingItemId));

        // Assert
        Assert.Equal(
            "Invoice can only be modified while it is in draft status.",
            exception.Message);

        Assert.Equal(originalCount, invoice.Items.Count);
    }

    [Fact]
    public void SetDiscount_ShouldThrowDomainException_WhenInvoiceIsNotDraft()
    {
        // Arrange
        var invoice = CreateIssuedInvoice();
        var originalDiscount = invoice.Discount;

        // Act
        var exception = Assert.Throws<DomainException>(
            () => invoice.SetDiscount(10m));

        // Assert
        Assert.Equal(
            "Invoice can only be modified while it is in draft status.",
            exception.Message);

        Assert.Equal(originalDiscount, invoice.Discount);
    }

    [Fact]
    public void SetTax_ShouldThrowDomainException_WhenInvoiceIsNotDraft()
    {
        // Arrange
        var invoice = CreateIssuedInvoice();
        var originalTax = invoice.Tax;

        // Act
        var exception = Assert.Throws<DomainException>(
            () => invoice.SetTax(10m));

        // Assert
        Assert.Equal(
            "Invoice can only be modified while it is in draft status.",
            exception.Message);

        Assert.Equal(originalTax, invoice.Tax);
    }


[Fact]
public void Issue_ShouldChangeStatusToIssued_WhenInvoiceHasValidItems()
    {
        // Arrange
        var invoice = CreateInvoiceWithItems();
        var beforeIssue = DateTime.UtcNow;

        // Act
        invoice.Issue();

        var afterIssue = DateTime.UtcNow;

        // Assert
        Assert.Equal(InvoiceStatus.Issued, invoice.Status);
        Assert.Equal(500m, invoice.SubTotal);
        Assert.Equal(500m, invoice.Total);
        Assert.NotNull(invoice.UpdatedAt);
        Assert.InRange(invoice.UpdatedAt.Value, beforeIssue, afterIssue);
    }

    [Fact]
    public void Issue_ShouldThrowDomainException_WhenInvoiceHasNoItems()
    {
        // Arrange
        var invoice = CreateInvoice();
        var originalUpdatedAt = invoice.UpdatedAt;

        // Act
        var exception = Assert.Throws<DomainException>(
            () => invoice.Issue());

        // Assert
        Assert.Equal(
            "Invoice must contain at least one item.",
            exception.Message);

        Assert.Equal(InvoiceStatus.Draft, invoice.Status);
        Assert.Equal(0m, invoice.Total);
        Assert.Empty(invoice.Items);
        Assert.Equal(originalUpdatedAt, invoice.UpdatedAt);
    }

    [Fact]
    public void Issue_ShouldThrowDomainException_WhenInvoiceTotalIsZero()
    {
        // Arrange
        var invoice = CreateInvoiceWithItems();

        // Subtotal = 500, discount = 500, total = 0.
        invoice.SetDiscount(500m);

        var originalUpdatedAt = invoice.UpdatedAt;

        // Act
        var exception = Assert.Throws<DomainException>(
            () => invoice.Issue());

        // Assert
        Assert.Equal(
            "Invoice total must be greater than zero.",
            exception.Message);

        Assert.Equal(InvoiceStatus.Draft, invoice.Status);
        Assert.Equal(500m, invoice.SubTotal);
        Assert.Equal(500m, invoice.Discount);
        Assert.Equal(0m, invoice.Total);
        Assert.Equal(originalUpdatedAt, invoice.UpdatedAt);
    }

    [Fact]
    public void Issue_ShouldThrowDomainException_WhenInvoiceIsAlreadyIssued()
    {
        // Arrange
        var invoice = CreateIssuedInvoice();
        var originalUpdatedAt = invoice.UpdatedAt;

        // Act
        var exception = Assert.Throws<DomainException>(
            () => invoice.Issue());

        // Assert
        Assert.Equal(
            "Invoice can only be modified while it is in draft status.",
            exception.Message);

        Assert.Equal(InvoiceStatus.Issued, invoice.Status);
        Assert.Equal(500m, invoice.Total);
        Assert.Equal(originalUpdatedAt, invoice.UpdatedAt);
    }


 private static Payment CreatePayment(
    Guid invoiceId,
    decimal amount,
    string idempotencyKey = "test-payment-001")
    {
        return new Payment(
            invoiceId,
            amount,
            PaymentMethod.Cash,
            idempotencyKey);
    }

    [Fact]
    public void RegisterPayment_ShouldAddPayment_WhenInvoiceIsIssued()
    {
        // Arrange
        var invoice = CreateIssuedInvoice();
        var payment = CreatePayment(invoice.Id, 200m);
        var beforeUpdate = DateTime.UtcNow;

        // Act
        invoice.RegisterPayment(payment);

        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.Single(invoice.Payments);
        Assert.Contains(payment, invoice.Payments);

        Assert.Equal(InvoiceStatus.Issued, invoice.Status);
        Assert.Equal(0m, invoice.PaidAmount);
        Assert.Equal(500m, invoice.RemainingAmount);
        Assert.NotNull(invoice.UpdatedAt);
        Assert.InRange(invoice.UpdatedAt.Value, beforeUpdate, afterUpdate);
    }

    [Fact]
    public void RegisterPayment_ShouldThrowDomainException_WhenPaymentIsNull()
    {
        // Arrange
        var invoice = CreateIssuedInvoice();
        var originalUpdatedAt = invoice.UpdatedAt;

        // Act
        var exception = Assert.Throws<DomainException>(
            () => invoice.RegisterPayment(null!));

        // Assert
        Assert.Equal("Payment is required.", exception.Message);
        Assert.Empty(invoice.Payments);
        Assert.Equal(originalUpdatedAt, invoice.UpdatedAt);
    }

    [Fact]
    public void RegisterPayment_ShouldThrowDomainException_WhenInvoiceIsDraft()
    {
        // Arrange
        var invoice = CreateInvoice();
        var payment = CreatePayment(invoice.Id, 100m);
        var originalUpdatedAt = invoice.UpdatedAt;

        // Act
        var exception = Assert.Throws<DomainException>(
            () => invoice.RegisterPayment(payment));

        // Assert
        Assert.Equal(
            "Payment cannot be registered for this invoice.",
            exception.Message);

        Assert.Empty(invoice.Payments);
        Assert.Equal(InvoiceStatus.Draft, invoice.Status);
        Assert.Equal(originalUpdatedAt, invoice.UpdatedAt);
    }

    [Fact]
    public void RegisterPayment_ShouldThrowDomainException_WhenInvoiceIsCancelled()
    {
        // Arrange
        var invoice = CreateInvoice();
        invoice.Cancel();

        var payment = CreatePayment(invoice.Id, 100m);
        var originalUpdatedAt = invoice.UpdatedAt;

        // Act
        var exception = Assert.Throws<DomainException>(
            () => invoice.RegisterPayment(payment));

        // Assert
        Assert.Equal(
            "Payment cannot be registered for this invoice.",
            exception.Message);

        Assert.Empty(invoice.Payments);
        Assert.Equal(InvoiceStatus.Cancelled, invoice.Status);
        Assert.Equal(originalUpdatedAt, invoice.UpdatedAt);
    }

    [Fact]
    public void RegisterPayment_ShouldThrowDomainException_WhenInvoiceIsAlreadyPaid()
    {
        // Arrange
        var invoice = CreateIssuedInvoice();

        var firstPayment = CreatePayment(
            invoice.Id,
            500m,
            "test-payment-full");

        invoice.RegisterPayment(firstPayment);
        firstPayment.MarkAsSucceeded("gateway-tx-full");
        invoice.RefreshPaymentStatus();

        Assert.Equal(InvoiceStatus.Paid, invoice.Status);

        var secondPayment = CreatePayment(
            invoice.Id,
            10m,
            "test-payment-extra");

        var originalUpdatedAt = invoice.UpdatedAt;

        // Act
        var exception = Assert.Throws<DomainException>(
            () => invoice.RegisterPayment(secondPayment));

        // Assert
        Assert.Equal(
            "Invoice is already fully paid.",
            exception.Message);

        Assert.Single(invoice.Payments);
        Assert.Equal(InvoiceStatus.Paid, invoice.Status);
        Assert.Equal(originalUpdatedAt, invoice.UpdatedAt);
    }

    [Fact]
    public void RegisterPayment_ShouldThrowDomainException_WhenPaymentExceedsRemainingAmount()
    {
        // Arrange
        var invoice = CreateIssuedInvoice();
        var payment = CreatePayment(invoice.Id, 501m);
        var originalUpdatedAt = invoice.UpdatedAt;

        // Act
        var exception = Assert.Throws<DomainException>(
            () => invoice.RegisterPayment(payment));

        // Assert
        Assert.Equal(
            "Payment amount exceeds the remaining invoice amount.",
            exception.Message);

        Assert.Empty(invoice.Payments);
        Assert.Equal(500m, invoice.RemainingAmount);
        Assert.Equal(originalUpdatedAt, invoice.UpdatedAt);
    }

    [Fact]
    public void PaidAmount_ShouldIncludeOnlySuccessfulPayments()
    {
        // Arrange
        var invoice = CreateIssuedInvoice();

        var succeededPayment = CreatePayment(
            invoice.Id,
            100m,
            "test-payment-success");

        var pendingPayment = CreatePayment(
            invoice.Id,
            50m,
            "test-payment-pending");

        var failedPayment = CreatePayment(
            invoice.Id,
            25m,
            "test-payment-failed");

        invoice.RegisterPayment(succeededPayment);
        invoice.RegisterPayment(pendingPayment);
        invoice.RegisterPayment(failedPayment);

        succeededPayment.MarkAsSucceeded("gateway-tx-success");
        pendingPayment.MarkAsCancelled();
        failedPayment.MarkAsFailed();

        // Act & Assert
        Assert.Equal(100m, invoice.PaidAmount);
        Assert.Equal(400m, invoice.RemainingAmount);
    }

    [Fact]
    public void RegisterPayment_ShouldAcceptPaymentEqualToRemainingAmount()
    {
        // Arrange
        var invoice = CreateIssuedInvoice();
        var payment = CreatePayment(invoice.Id, 500m);

        // Act
        invoice.RegisterPayment(payment);

        // Assert
        Assert.Single(invoice.Payments);
        Assert.Equal(500m, invoice.RemainingAmount);
    }

[Fact]
public void RefreshPaymentStatus_ShouldRemainIssued_WhenNoPaymentSucceeded()
    {
        // Arrange
        var invoice = CreateIssuedInvoice();

        var payment = CreatePayment(invoice.Id, 100m);
        invoice.RegisterPayment(payment);

        // Payment remains Pending.
        var beforeRefresh = DateTime.UtcNow;

        // Act
        invoice.RefreshPaymentStatus();

        var afterRefresh = DateTime.UtcNow;

        // Assert
        Assert.Equal(InvoiceStatus.Issued, invoice.Status);
        Assert.Equal(0m, invoice.PaidAmount);
        Assert.Equal(500m, invoice.RemainingAmount);
        Assert.NotNull(invoice.UpdatedAt);
        Assert.InRange(invoice.UpdatedAt.Value, beforeRefresh, afterRefresh);
    }

    [Fact]
    public void RefreshPaymentStatus_ShouldSetPartiallyPaid_WhenOnlyPartOfInvoiceIsPaid()
    {
        // Arrange
        var invoice = CreateIssuedInvoice();

        var payment = CreatePayment(invoice.Id, 200m);
        invoice.RegisterPayment(payment);
        payment.MarkAsSucceeded("gateway-partial-001");

        // Act
        invoice.RefreshPaymentStatus();

        // Assert
        Assert.Equal(InvoiceStatus.PartiallyPaid, invoice.Status);
        Assert.Equal(200m, invoice.PaidAmount);
        Assert.Equal(300m, invoice.RemainingAmount);
    }

    [Fact]
    public void RefreshPaymentStatus_ShouldSetPaid_WhenInvoiceIsFullyPaid()
    {
        // Arrange
        var invoice = CreateIssuedInvoice();

        var payment = CreatePayment(invoice.Id, 500m);
        invoice.RegisterPayment(payment);
        payment.MarkAsSucceeded("gateway-full-001");

        // Act
        invoice.RefreshPaymentStatus();

        // Assert
        Assert.Equal(InvoiceStatus.Paid, invoice.Status);
        Assert.Equal(500m, invoice.PaidAmount);
        Assert.Equal(0m, invoice.RemainingAmount);
    }

    [Fact]
    public void RefreshPaymentStatus_ShouldRemainCancelled_WhenInvoiceIsCancelled()
    {
        // Arrange
        var invoice = CreateInvoice();
        invoice.Cancel();

        var originalUpdatedAt = invoice.UpdatedAt;

        // Act
        invoice.RefreshPaymentStatus();

        // Assert
        Assert.Equal(InvoiceStatus.Cancelled, invoice.Status);
        Assert.Equal(originalUpdatedAt, invoice.UpdatedAt);
    }

    
[Fact]
public void RefreshPaymentStatus_ShouldSetPartiallyRefunded_WhenPartOfPaymentIsRefunded()
    {
        // Arrange
        var invoice = CreateIssuedInvoice();

        var payment = CreatePayment(invoice.Id, 500m);
        invoice.RegisterPayment(payment);
        payment.MarkAsSucceeded("gateway-payment-refund-001");

        invoice.RefreshPaymentStatus();

        Assert.Equal(InvoiceStatus.Paid, invoice.Status);

        var refund = new Refund(
            payment.Id,
            100m,
            "test-refund-partial-001");

        payment.RegisterRefund(refund);

        // Refund is not counted until it succeeds.
        Assert.Equal(0m, invoice.RefundedAmount);
        Assert.Equal(500m, invoice.NetPaidAmount);

        refund.MarkAsSucceeded("gateway-refund-partial-001");

        // Act
        invoice.RefreshPaymentStatus();

        // Assert
        Assert.Equal(500m, invoice.PaidAmount);
        Assert.Equal(100m, invoice.RefundedAmount);
        Assert.Equal(400m, invoice.NetPaidAmount);
        Assert.Equal(100m, invoice.RemainingAmount);
        Assert.Equal(InvoiceStatus.PartiallyRefunded, invoice.Status);
    }

    [Fact]
    public void RefreshPaymentStatus_ShouldSetRefunded_WhenEntirePaymentIsRefunded()
    {
        // Arrange
        var invoice = CreateIssuedInvoice();

        var payment = CreatePayment(invoice.Id, 500m);
        invoice.RegisterPayment(payment);
        payment.MarkAsSucceeded("gateway-payment-refund-002");

        invoice.RefreshPaymentStatus();

        var refund = new Refund(
            payment.Id,
            500m,
            "test-refund-full-001");

        payment.RegisterRefund(refund);
        refund.MarkAsSucceeded("gateway-refund-full-001");

        // Act
        invoice.RefreshPaymentStatus();

        // Assert
        Assert.Equal(500m, invoice.PaidAmount);
        Assert.Equal(500m, invoice.RefundedAmount);
        Assert.Equal(0m, invoice.NetPaidAmount);
        Assert.Equal(500m, invoice.RemainingAmount);
        Assert.Equal(InvoiceStatus.Refunded, invoice.Status);
    }

    [Fact]
    public void RefreshPaymentStatus_ShouldNotCountFailedRefund()
    {
        // Arrange
        var invoice = CreateIssuedInvoice();

        var payment = CreatePayment(invoice.Id, 500m);
        invoice.RegisterPayment(payment);
        payment.MarkAsSucceeded("gateway-payment-refund-003");

        invoice.RefreshPaymentStatus();

        var refund = new Refund(
            payment.Id,
            100m,
            "test-refund-failed-001");

        payment.RegisterRefund(refund);
        refund.MarkAsFailed();

        // Act
        invoice.RefreshPaymentStatus();

        // Assert
        Assert.Equal(500m, invoice.PaidAmount);
        Assert.Equal(0m, invoice.RefundedAmount);
        Assert.Equal(500m, invoice.NetPaidAmount);
        Assert.Equal(0m, invoice.RemainingAmount);
        Assert.Equal(InvoiceStatus.Paid, invoice.Status);
    }

 
[Fact]
public void Cancel_ShouldSetStatusToCancelled_WhenInvoiceHasNoSuccessfulPayments()
    {
        // Arrange
        var invoice = CreateIssuedInvoice();
        var beforeCancel = DateTime.UtcNow;

        // Act
        invoice.Cancel();

        var afterCancel = DateTime.UtcNow;

        // Assert
        Assert.Equal(InvoiceStatus.Cancelled, invoice.Status);
        Assert.NotNull(invoice.UpdatedAt);
        Assert.InRange(invoice.UpdatedAt.Value, beforeCancel, afterCancel);
    }

    [Fact]
    public void Cancel_ShouldAllowCancellation_WhenPaymentIsPending()
    {
        // Arrange
        var invoice = CreateIssuedInvoice();

        var pendingPayment = CreatePayment(invoice.Id, 100m);
        invoice.RegisterPayment(pendingPayment);

        // Act
        invoice.Cancel();

        // Assert
        Assert.Equal(InvoiceStatus.Cancelled, invoice.Status);
        Assert.Single(invoice.Payments);
        Assert.Equal(PaymentStatus.Pending, pendingPayment.Status);
        Assert.Equal(0m, invoice.PaidAmount);
    }

    [Fact]
    public void Cancel_ShouldThrowDomainException_WhenInvoiceHasSuccessfulPayments()
    {
        // Arrange
        var invoice = CreateIssuedInvoice();

        var payment = CreatePayment(invoice.Id, 100m);
        invoice.RegisterPayment(payment);
        payment.MarkAsSucceeded("gateway-cancel-001");

        var originalUpdatedAt = invoice.UpdatedAt;

        // Act
        var exception = Assert.Throws<DomainException>(
            () => invoice.Cancel());

        // Assert
        Assert.Equal(
            "An invoice with payments cannot be cancelled.",
            exception.Message);

        Assert.Equal(InvoiceStatus.Issued, invoice.Status);
        Assert.Equal(100m, invoice.PaidAmount);
        Assert.Equal(originalUpdatedAt, invoice.UpdatedAt);
    }

    [Fact]
    public void Cancel_ShouldThrowDomainException_WhenInvoiceIsAlreadyCancelled()
    {
        // Arrange
        var invoice = CreateInvoice();
        invoice.Cancel();

        var originalUpdatedAt = invoice.UpdatedAt;

        // Act
        var exception = Assert.Throws<DomainException>(
            () => invoice.Cancel());

        // Assert
        Assert.Equal(
            "Invoice is already cancelled.",
            exception.Message);

        Assert.Equal(InvoiceStatus.Cancelled, invoice.Status);
        Assert.Equal(originalUpdatedAt, invoice.UpdatedAt);
    }

   
[Fact]
public void RegisterPayment_ShouldThrowDomainException_WhenPaymentBelongsToAnotherInvoice()
    {
        // Arrange
        var invoice = CreateIssuedInvoice();

        var payment = CreatePayment(
            Guid.NewGuid(),
            100m,
            "wrong-invoice-payment-001");

        // Act
        var exception = Assert.Throws<DomainException>(
            () => invoice.RegisterPayment(payment));

        // Assert
        Assert.Equal(
            "Payment does not belong to this invoice.",
            exception.Message);

        Assert.Empty(invoice.Payments);
        Assert.Equal(InvoiceStatus.Issued, invoice.Status);
        Assert.Equal(0m, invoice.PaidAmount);
    }


    [Fact]
    public void RegisterPayment_ShouldRejectPayment_WhenPendingPaymentsExceedInvoiceTotal()
    {
        // Arrange
        var invoice = CreateIssuedInvoice(); // Total = 500

        var firstPayment = CreatePayment(
            invoice.Id,
            400m,
            "pending-payment-001");

        var secondPayment = CreatePayment(
            invoice.Id,
            400m,
            "pending-payment-002");

        invoice.RegisterPayment(firstPayment);

        // Act
        var exception = Assert.Throws<DomainException>(
            () => invoice.RegisterPayment(secondPayment));

        // Assert
        Assert.Equal(
            "Payment amount exceeds the remaining invoice amount.",
            exception.Message);

        Assert.Single(invoice.Payments);
    }

    [Fact]
    public void RegisterPayment_ShouldAllowNewPayment_WhenPreviousPaymentFailed()
    {
        // Arrange
        var invoice = CreateIssuedInvoice();

        var failedPayment = CreatePayment(
            invoice.Id,
            400m,
            "failed-payment-001");

        invoice.RegisterPayment(failedPayment);
        failedPayment.MarkAsFailed();

        var newPayment = CreatePayment(
            invoice.Id,
            400m,
            "retry-payment-002");

        // Act
        invoice.RegisterPayment(newPayment);

        // Assert
        Assert.Equal(2, invoice.Payments.Count);
        Assert.Equal(PaymentStatus.Failed, failedPayment.Status);
        Assert.Equal(PaymentStatus.Pending, newPayment.Status);
        Assert.Equal(400m, invoice.PendingPaymentAmount);
    }


    [Fact]
    public void RegisterPayment_ShouldReserveAmount_WhenPaymentAttemptIsUnknown()
    {
        // Arrange
        var invoice = CreateIssuedInvoice();

        var payment = CreatePayment(
            invoice.Id,
            400m,
            "unknown-payment-001");

        var attempt = new PaymentAttempt(
            payment.Id,
            "unknown-attempt-001");

        attempt.MarkAsUnknown("Gateway timeout");
        payment.AddAttempt(attempt);

        invoice.RegisterPayment(payment);

        var secondPayment = CreatePayment(
            invoice.Id,
            200m,
            "unknown-payment-002");

        // Act
        var exception = Assert.Throws<DomainException>(
            () => invoice.RegisterPayment(secondPayment));

        // Assert
        Assert.Equal(
            "Payment amount exceeds the remaining invoice amount.",
            exception.Message);

        Assert.Single(invoice.Payments);
        Assert.Equal(PaymentStatus.Pending, payment.Status);
        Assert.Equal(PaymentAttemptStatus.Unknown, attempt.Status);
        Assert.Equal(400m, invoice.PendingPaymentAmount);
    }






}

