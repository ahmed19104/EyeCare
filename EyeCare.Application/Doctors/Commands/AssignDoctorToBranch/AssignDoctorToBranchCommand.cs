using MediatR;

namespace EyeCare.Application.Doctors.Commands.AssignDoctorToBranch;

public sealed record AssignDoctorToBranchCommand(
    Guid DoctorId,
    Guid BranchId) : IRequest;
