using FluentValidation;

namespace EyeCare.Application.Doctors.Commands.CreateDoctor
{
    public sealed class CreateDoctorCommandValidator
        : AbstractValidator<CreateDoctorCommand>
    {
        public CreateDoctorCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .MaximumLength(20);

            RuleFor(x => x.Specialty)
                .NotEmpty()
                .MaximumLength(150);
        }
    }
}
