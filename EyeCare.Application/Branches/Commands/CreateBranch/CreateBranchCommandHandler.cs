using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;
using MediatR;

namespace EyeCare.Application.Branches.Commands.CreateBranch;

public sealed class CreateBranchCommandHandler
    : IRequestHandler<CreateBranchCommand, Guid>
{
    private readonly IRepository<Branch> _branchRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBranchCommandHandler(
        IRepository<Branch> branchRepository,
        IUnitOfWork unitOfWork)
    {
        _branchRepository = branchRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateBranchCommand request,
        CancellationToken cancellationToken)
    {
        var branch = new Branch(
            request.Name,
            request.Address,
            request.PhoneNumber);

        await _branchRepository.AddAsync(
            branch,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return branch.Id;
    }
}
