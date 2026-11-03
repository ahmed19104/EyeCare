using EyeCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EyeCare.Infrastructure.Persistence.Configurations;

public class MedicalRecordConfiguration
    : IEntityTypeConfiguration<MedicalRecord>
{
    public void Configure(
        EntityTypeBuilder<MedicalRecord> builder)
    {
        builder.ToTable("MedicalRecords");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Diagnosis)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(5000);

        builder.Property(x => x.TreatmentPlan)
            .HasMaxLength(5000);

        builder.Property(x => x.FollowUpDate);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.HasOne(x => x.Appointment)
            .WithMany()
            .HasForeignKey(x => x.AppointmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Patient)
            .WithMany()
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Doctor)
            .WithMany()
            .HasForeignKey(x => x.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.AppointmentId)
            .IsUnique();

        builder.HasIndex(x => new
        {
            x.PatientId,
            x.CreatedAt
        });
    }
}