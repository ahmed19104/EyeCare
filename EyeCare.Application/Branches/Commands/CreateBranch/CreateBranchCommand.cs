using MediatR;

namespace EyeCare.Application.Branches.Commands.CreateBranch
{
    public sealed record CreateBranchCommand(
        string Name,
        string Address,
        string PhoneNumber) : IRequest<Guid>;
}
