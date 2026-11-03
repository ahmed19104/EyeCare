using FluentValidation;

namespace EyeCare.Application.Patients.Commands.CreatePatient;

public sealed class CreatePatientCommandValidator
    : AbstractValidator<CreatePatientCommand>
{
    public CreatePatientCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.DateOfBirth)
            .LessThanOrEqualTo(DateTime.UtcNow.Date);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.NationalId)
            .MaximumLength(20)
            .When(x => !string.IsNullOrWhiteSpace(x.NationalId));

        RuleFor(x => x.Gender)
            .IsInEnum();
    }
}
