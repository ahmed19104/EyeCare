using FluentValidation;

namespace EyeCare.Application.Doctors.Commands.CreateDoctorSchedule;

public sealed class CreateDoctorScheduleCommandValidator
    : AbstractValidator<CreateDoctorScheduleCommand>
{
    public CreateDoctorScheduleCommandValidator()
    {
        RuleFor(x => x.DoctorId)
            .NotEmpty();

        RuleFor(x => x.BranchId)
            .NotEmpty();

        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime);

        RuleFor(x => x.SlotDurationMinutes)
            .GreaterThan(0)
            .LessThanOrEqualTo(240);
    }
}
