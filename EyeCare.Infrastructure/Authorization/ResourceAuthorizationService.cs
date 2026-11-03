using EyeCare.Application.Abstractions.Authorization;
using EyeCare.Application.Abstractions.Security;
using EyeCare.Application.Exceptions;
using EyeCare.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EyeCare.Infrastructure.Authorization;

public sealed class ResourceAuthorizationService
    : IResourceAuthorizationService
{
    private readonly AppDbContext _context;
    private readonly IBranchAuthorizationService
        _branchAuthorizationService;
    private readonly IUserBranchService
    _userBranchService;
    private readonly ICurrentUser
    _currentUser;

    public ResourceAuthorizationService(
        AppDbContext context,
        IBranchAuthorizationService branchAuthorizationService, IUserBranchService userBranchService, ICurrentUser currentUser)
    {
        _context = context;
        _branchAuthorizationService =
            branchAuthorizationService;
        _userBranchService = userBranchService;
        _currentUser = currentUser;
    }

    public async Task EnsureCanAccessAppointmentAsync(
        Guid appointmentId,
        CancellationToken cancellationToken = default)
    {
        var branchId =
            await _context.Appointments
                .Where(x => x.Id == appointmentId)
                .Select(x => (Guid?)x.BranchId)
                .FirstOrDefaultAsync(
                    cancellationToken);

        if (branchId is null)
        {
            throw new NotFoundException(
                "Appointment",
                appointmentId);
        }

        await _branchAuthorizationService
            .EnsureCanAccessBranchAsync(
                branchId.Value,
                cancellationToken);
    }

    public async Task EnsureCanAccessMedicalRecordAsync(
        Guid medicalRecordId,
        CancellationToken cancellationToken = default)
    {
        var branchId =
            await _context.MedicalRecords
                .Where(x => x.Id == medicalRecordId)
                .Select(x => (Guid?)x.Appointment.BranchId)
                .FirstOrDefaultAsync(
                    cancellationToken);

        if (branchId is null)
        {
            throw new NotFoundException(
                "Medical record",
                medicalRecordId);
        }

        await _branchAuthorizationService
            .EnsureCanAccessBranchAsync(
                branchId.Value,
                cancellationToken);
    }

    public async Task EnsureCanAccessPatientAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        var hasAppointment =
            await _context.Appointments
                .AnyAsync(
                    x => x.PatientId == patientId &&
                         x.Status != Domain.Enums.AppointmentStatus.Cancelled &&
                         x.Status != Domain.Enums.AppointmentStatus.NoShow,
                    cancellationToken);

        if (!hasAppointment)
        {
            throw new ForbiddenException(
                "You do not have access to this patient.");
        }

        var branchIds =
            await _context.Appointments
                .Where(
                    x => x.PatientId == patientId &&
                         x.Status != Domain.Enums.AppointmentStatus.Cancelled &&
                         x.Status != Domain.Enums.AppointmentStatus.NoShow)
                .Select(x => x.BranchId)
                .Distinct()
                .ToListAsync(cancellationToken);

        foreach (var branchId in branchIds)
        {
            try
            {
                await _branchAuthorizationService
                    .EnsureCanAccessBranchAsync(
                        branchId,
                        cancellationToken);

                return;
            }
            catch (ForbiddenException)
            {
                // Try the patient's next branch.
            }
        }

        throw new ForbiddenException(
            "You do not have access to this patient.");
    }

    public async Task<IReadOnlyList<Guid>>
    GetAccessibleMedicalRecordBranchIdsAsync(
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_currentUser.UserId))
        {
            throw new UnauthorizedException(
                "You must be authenticated.");
        }
        var branchIds =
            await _userBranchService
                .GetUserBranchIdsAsync(
                    _currentUser.UserId!,
                    cancellationToken);

        return branchIds;
    }
}