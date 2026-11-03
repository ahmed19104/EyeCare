using EyeCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EyeCare.Infrastructure.Persistence.Configurations;

public sealed class RefundConfiguration
    : IEntityTypeConfiguration<Refund>
{
    public void Configure(EntityTypeBuilder<Refund> builder)
    {
        builder.ToTable("Refunds");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.IdempotencyKey)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.ExternalTransactionId)
            .HasMaxLength(200);

        builder.HasIndex(x => x.IdempotencyKey)
            .IsUnique().HasDatabaseName("UX_Refunds_IdempotencyKey");

        builder.HasMany(x => x.Attempts)
            .WithOne(x => x.Refund)
            .HasForeignKey(x => x.RefundId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.PaymentId);

        builder.HasOne(x => x.Payment)
            .WithMany(x => x.Refunds)
            .HasForeignKey(x => x.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.RowVersion)
    .IsRowVersion();
    }
}