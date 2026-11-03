using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Abstractions.Security;
using EyeCare.Domain.Entities;
using EyeCare.Domain.Enums;
using EyeCare.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;



namespace EyeCare.Infrastructure.Persistence
{
    public class AppDbContext
    : IdentityDbContext<ApplicationUser>, IUnitOfWork
    {
        private readonly ICurrentUser _currentUser;
        public AppDbContext(
     DbContextOptions<AppDbContext> options,
     ICurrentUser currentUser)
     : base(options)
        {
            _currentUser = currentUser;
        }

        public DbSet<Patient> Patients => Set<Patient>();

        public DbSet<Doctor> Doctors => Set<Doctor>();

        public DbSet<Branch> Branches => Set<Branch>();
        public DbSet<DoctorSchedule> DoctorSchedules =>
    Set<DoctorSchedule>();
        public DbSet<PaymentWebhookEvent>
    PaymentWebhookEvents
    => Set<PaymentWebhookEvent>();
        public DbSet<DoctorBranch> DoctorBranches => Set<DoctorBranch>();
        public DbSet<RolePermission> RolePermissions =>
    Set<RolePermission>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<UserBranch> UserBranches => Set<UserBranch>();
        public DbSet<MedicalRecord> MedicalRecords =>
    Set<MedicalRecord>();
        public DbSet<AuditLog> AuditLogs =>
    Set<AuditLog>();
        public DbSet<RefundAttempt>
    RefundAttempts
    => Set<RefundAttempt>();
        public DbSet<Refund> Refunds
    => Set<Refund>();

        public DbSet<RefundWebhookEvent> RefundWebhookEvents
    => Set<RefundWebhookEvent>();
        public DbSet<Invoice> Invoices => Set<Invoice>();



        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(AppDbContext).Assembly);
        }
        public override async Task<int> SaveChangesAsync(
      CancellationToken cancellationToken = default)
        {
            try
            {
                CreateAuditLogs();

                return await base.SaveChangesAsync(
                    cancellationToken);
            }
            catch (DbUpdateException exception)
            {
                DatabaseExceptionHandler.Handle(exception);
                throw;
            }
        }

        private void CreateAuditLogs()
        {
            var auditEntries = ChangeTracker
                .Entries<MedicalRecord>()
                .Where(entry =>
                    entry.State == EntityState.Added ||
                    entry.State == EntityState.Modified)
                .ToList();

            foreach (var entry in auditEntries)
            {
                var changes = new Dictionary<string, object?>();

                foreach (var property in entry.Properties)
                {
                    if (entry.State == EntityState.Added)
                    {
                        changes[property.Metadata.Name] =
                            property.CurrentValue;

                        continue;
                    }

                    if (entry.Property(property.Metadata.Name)
                            .IsModified)
                    {
                        changes[property.Metadata.Name] = new
                        {
                            OldValue = property.OriginalValue,
                            NewValue = property.CurrentValue
                        };
                    }
                }

                if (changes.Count == 0)
                    continue;

                var action =
                    entry.State == EntityState.Added
                        ? AuditAction.Created
                        : AuditAction.Updated;

                var auditLog =
                    new AuditLog(
                        nameof(MedicalRecord),
                        entry.Entity.Id,
                        action,
                        _currentUser.UserId,
                        JsonSerializer.Serialize(changes));

                AuditLogs.Add(auditLog);
            }
        }
    }
}