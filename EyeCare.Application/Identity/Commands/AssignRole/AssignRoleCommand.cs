using MediatR;

namespace EyeCare.Application.Identity.Commands.AssignRole;

public sealed record AssignRoleCommand(
    string UserId,
    string Role)
    : IRequest;
