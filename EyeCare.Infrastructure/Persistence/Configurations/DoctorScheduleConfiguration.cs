using EyeCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EyeCare.Infrastructure.Persistence.Configurations;

public class DoctorScheduleConfiguration
    : IEntityTypeConfiguration<DoctorSchedule>
{
    public void Configure(
        EntityTypeBuilder<DoctorSchedule> builder)
    {
        builder.ToTable("DoctorSchedules");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Day)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.StartTime)
            .IsRequired();

        builder.Property(x => x.EndTime)
            .IsRequired();

        builder.Property(x => x.SlotDurationMinutes)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.HasOne(x => x.DoctorBranch)
            .WithMany()
            .HasForeignKey(x => x.DoctorBranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.DoctorBranchId,
            x.Day
        })
        .IsUnique();
    }
}