using EyeCare.Application.Abstractions.Identity;
using FluentValidation;

namespace EyeCare.Application.Identity.Commands.AssignRole;

public sealed class AssignRoleValidator
    : AbstractValidator<AssignRoleCommand>
{
    public AssignRoleValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(role => Roles.All.Contains(role))
            .WithMessage("Invalid role.");
    }
}
