using EyeCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EyeCare.Infrastructure.Persistence.Configurations;

public sealed class RefundAttemptConfiguration
    : IEntityTypeConfiguration<RefundAttempt>
{
    public void Configure(
        EntityTypeBuilder<RefundAttempt> builder)
    {
        builder.ToTable("RefundAttempts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.AttemptKey)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.ExternalTransactionId)
            .HasMaxLength(200);

        builder.Property(x => x.FailureReason)
            .HasMaxLength(1000);

        builder.Property(x => x.CompletedAt);

        builder.HasIndex(x => x.AttemptKey)
            .IsUnique().HasDatabaseName("UX_RefundAttempts_AttemptKey");

        builder.HasIndex(x => x.RefundId);

        builder.HasOne(x => x.Refund)
            .WithMany(x => x.Attempts)
            .HasForeignKey(x => x.RefundId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
