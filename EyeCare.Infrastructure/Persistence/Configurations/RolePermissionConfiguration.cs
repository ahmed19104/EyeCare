using EyeCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EyeCare.Infrastructure.Persistence.Configurations;

public class RolePermissionConfiguration
    : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(
        EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");

        builder.HasKey(x => new
        {
            x.RoleId,
            x.Permission
        });

        builder.Property(x => x.RoleId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(x => x.Permission)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasOne<Microsoft.AspNetCore.Identity.IdentityRole>()
            .WithMany()
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}