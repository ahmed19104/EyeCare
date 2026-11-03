
using EyeCare.Domain.Entities;
using EyeCare.Domain.Exceptions;
using Xunit;

namespace EyeCare.Domain.Tests.Entities;

public class InvoiceItemTests
{
    [Fact]
    public void Constructor_ShouldCreateInvoiceItem_WithValidData()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var description = "Eye examination";
        decimal quantity = 2;
        decimal unitPrice = 150m;

        var before = DateTime.UtcNow;

        // Act
        var item = new InvoiceItem(
            invoiceId,
            description,
            quantity,
            unitPrice);

        var after = DateTime.UtcNow;

        // Assert
        Assert.NotEqual(Guid.Empty, item.Id);
        Assert.Equal(invoiceId, item.InvoiceId);
        Assert.Equal(description, item.Description);
        Assert.Equal(quantity, item.Quantity);
        Assert.Equal(unitPrice, item.UnitPrice);
        Assert.Equal(300m, item.Total);
        Assert.InRange(item.CreatedAt, before, after);
    }

    [Fact]
    public void Constructor_ShouldGenerateUniqueIds()
    {
        var invoiceId = Guid.NewGuid();

        var first = CreateInvoiceItem(invoiceId: invoiceId);
        var second = CreateInvoiceItem(invoiceId: invoiceId);

        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenInvoiceIdIsEmpty()
    {
        Assert.Throws<DomainException>(() =>
            CreateInvoiceItem(invoiceId: Guid.Empty));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Constructor_ShouldThrow_WhenDescriptionIsInvalid(
        string? description)
    {
        Assert.Throws<DomainException>(() =>
            CreateInvoiceItem(description: description!));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_ShouldThrow_WhenQuantityIsNotPositive(
        decimal quantity)
    {
        Assert.Throws<DomainException>(() =>
            CreateInvoiceItem(quantity: quantity));
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_ShouldThrow_WhenUnitPriceIsNegative(
        decimal unitPrice)
    {
        Assert.Throws<DomainException>(() =>
            CreateInvoiceItem(unitPrice: unitPrice));
    }

    [Fact]
    public void Constructor_ShouldAllowZeroUnitPrice()
    {
        var item = CreateInvoiceItem(
            quantity: 2,
            unitPrice: 0m);

        Assert.Equal(0m, item.UnitPrice);
        Assert.Equal(0m, item.Total);
    }

    [Fact]
    public void Constructor_ShouldCalculateTotal_Correctly()
    {
        var item = CreateInvoiceItem(
            quantity: 3,
            unitPrice: 125.50m);

        Assert.Equal(376.50m, item.Total);
    }

    [Fact]
    public void Constructor_ShouldCalculateFractionalQuantity_Correctly()
    {
        var item = CreateInvoiceItem(
            quantity: 1.5m,
            unitPrice: 100m);

        Assert.Equal(150m, item.Total);
    }

    [Fact]
    public void Constructor_ShouldCalculateFractionalUnitPrice_Correctly()
    {
        var item = CreateInvoiceItem(
            quantity: 2,
            unitPrice: 12.25m);

        Assert.Equal(24.50m, item.Total);
    }

    [Fact]
    public void Constructor_ShouldAllowSmallPositiveQuantity()
    {
        var item = CreateInvoiceItem(
            quantity: 0.01m,
            unitPrice: 100m);

        Assert.Equal(0.01m, item.Quantity);
        Assert.Equal(1m, item.Total);
    }

    [Fact]
    public void Constructor_ShouldPreserveDescriptionWhitespace()
    {
        var item = CreateInvoiceItem(
            description: "  Eye examination  ");

        Assert.Equal("  Eye examination  ", item.Description);
    }

    [Fact]
    public void Constructor_ShouldAllowLargeValidAmounts()
    {
        var item = CreateInvoiceItem(
            quantity: 100000m,
            unitPrice: 100000m);

        Assert.Equal(10_000_000_000m, item.Total);
    }

    private static InvoiceItem CreateInvoiceItem(
        Guid? invoiceId = null,
        string description = "Eye examination",
        decimal quantity = 1m,
        decimal unitPrice = 100m)
    {
        return new InvoiceItem(
            invoiceId ?? Guid.NewGuid(),
            description,
            quantity,
            unitPrice);
    }
}
