using EyeCare.Domain.Identity;
using EyeCare.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeCare.Infrastructure.Persistence.Configurations
{
    public sealed class RefreshTokenConfiguration
     : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(x => x.TokenHash)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.CreatedAtUtc)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(x => x.ExpiresAtUtc)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(x => x.RevokedAtUtc)
                .HasColumnType("datetime2");

            builder.Property(x => x.RowVersion)
    .IsRowVersion();

            builder.Property(x => x.ReplacedByTokenHash)
                .HasMaxLength(128);

            builder.HasIndex(x => x.TokenHash)
                .IsUnique();

            builder.HasIndex(x => new { x.UserId, x.ExpiresAtUtc });

            builder.HasIndex(x => new { x.UserId, x.RevokedAtUtc });

            builder.HasOne<ApplicationUser>()
    .WithMany()
    .HasForeignKey(x => x.UserId)
    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
