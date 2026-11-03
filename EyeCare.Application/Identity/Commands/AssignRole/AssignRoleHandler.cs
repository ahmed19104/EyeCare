using EyeCare.Application.Abstractions.Identity;
using MediatR;

namespace EyeCare.Application.Identity.Commands.AssignRole;

public sealed class AssignRoleHandler
    : IRequestHandler<AssignRoleCommand>
{
    private readonly IRoleService _roleService;

    public AssignRoleHandler(
        IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task Handle(
        AssignRoleCommand request,
        CancellationToken cancellationToken)
    {
        await _roleService.AssignRoleAsync(
            request.UserId,
            request.Role,
            cancellationToken);
    }
}
