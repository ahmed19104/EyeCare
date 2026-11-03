

using EyeCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EyeCare.Infrastructure.Persistence.Configurations
{
    public class DoctorBranchConfiguration
        : IEntityTypeConfiguration<DoctorBranch>
    {
        public void Configure(
            EntityTypeBuilder<DoctorBranch> builder)
        {
            builder.ToTable("DoctorBranches");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasOne(x => x.Doctor)
                .WithMany(x => x.DoctorBranches)
                .HasForeignKey(x => x.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Branch)
                .WithMany(x => x.DoctorBranches)
                .HasForeignKey(x => x.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new
            {
                x.DoctorId,
                x.BranchId
            })
            .IsUnique();
        }
    }
}
