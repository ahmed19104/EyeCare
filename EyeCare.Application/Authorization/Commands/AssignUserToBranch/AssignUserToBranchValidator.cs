using FluentValidation;

namespace EyeCare.Application.Authorization.Commands.AssignUserToBranch;

public sealed class AssignUserToBranchValidator
    : AbstractValidator<AssignUserToBranchCommand>
{
    public AssignUserToBranchValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.BranchId)
            .NotEmpty();
    }
}