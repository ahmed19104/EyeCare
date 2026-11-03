using EyeCare.Application.Abstractions.Authorization;
using MediatR;

namespace EyeCare.Application.Authorization.Commands.AssignUserToBranch;

public sealed class AssignUserToBranchHandler
    : IRequestHandler<AssignUserToBranchCommand>
{
    private readonly IUserBranchService _userBranchService;

    public AssignUserToBranchHandler(
        IUserBranchService userBranchService)
    {
        _userBranchService = userBranchService;
    }

    public async Task Handle(
        AssignUserToBranchCommand request,
        CancellationToken cancellationToken)
    {
        await _userBranchService.AssignUserToBranchAsync(
            request.UserId,
            request.BranchId,
            cancellationToken);
    }
}