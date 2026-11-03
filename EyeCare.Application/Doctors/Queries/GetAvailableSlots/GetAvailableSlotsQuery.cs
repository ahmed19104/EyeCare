using MediatR;

namespace EyeCare.Application.Doctors.Queries.GetAvailableSlots;

public sealed record GetAvailableSlotsQuery(
    Guid DoctorId,
    Guid BranchId,
    DateOnly Date) : IRequest<IReadOnlyList<TimeSpan>>;
