using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Doctors.Commands.AssignDoctorToBranch.Specifications;
using EyeCare.Application.Exceptions;
using EyeCare.Domain.Entities;
using MediatR;

namespace EyeCare.Application.Doctors.Commands.AssignDoctorToBranch;

public sealed class AssignDoctorToBranchCommandHandler
    : IRequestHandler<AssignDoctorToBranchCommand>
{
    private readonly IRepository<Doctor> _doctorRepository;
    private readonly IRepository<Branch> _branchRepository;
    private readonly IRepository<DoctorBranch> _doctorBranchRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignDoctorToBranchCommandHandler(
        IRepository<Doctor> doctorRepository,
        IRepository<Branch> branchRepository,
        IRepository<DoctorBranch> doctorBranchRepository,
        IUnitOfWork unitOfWork)
    {
        _doctorRepository = doctorRepository;
        _branchRepository = branchRepository;
        _doctorBranchRepository = doctorBranchRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        AssignDoctorToBranchCommand request,
        CancellationToken cancellationToken)
    {
        var doctor =
            await _doctorRepository.FirstOrDefaultAsync(
                new DoctorByIdSpecification(request.DoctorId),
                cancellationToken);

        if (doctor is null)
        {
            throw new NotFoundException(
                "Doctor",
                request.DoctorId);
        }

        if (!doctor.IsActive)
        {
            throw new ConflictException(
                "Doctor is not active.");
        }

        var branch =
            await _branchRepository.FirstOrDefaultAsync(
                new BranchByIdSpecification(request.BranchId),
                cancellationToken);

        if (branch is null)
        {
            throw new NotFoundException(
                "Branch",
                request.BranchId);
        }

        if (!branch.IsActive)
        {
            throw new ConflictException(
                "Branch is not active.");
        }

        var doctorBranch =
            await _doctorBranchRepository.FirstOrDefaultAsync(
                new DoctorBranchSpecification(
                    request.DoctorId,
                    request.BranchId),
                cancellationToken);

        if (doctorBranch is not null)
        {
            if (doctorBranch.IsActive)
            {
                throw new ConflictException(
                    "Doctor is already assigned to this branch.");
            }

            doctorBranch.Activate();

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return;
        }

        var newDoctorBranch = new DoctorBranch(
            request.DoctorId,
            request.BranchId);

        await _doctorBranchRepository.AddAsync(
            newDoctorBranch,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}
