using MediatR;

namespace EyeCare.Application.Authorization.Commands.AssignUserToBranch;

public sealed record AssignUserToBranchCommand(
    string UserId,
    Guid BranchId) : IRequest;