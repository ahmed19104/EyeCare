using FluentValidation;

namespace EyeCare.Application.MedicalRecords.Commands.CreateMedicalRecord;

public sealed class CreateMedicalRecordCommandValidator
    : AbstractValidator<CreateMedicalRecordCommand>
{
    public CreateMedicalRecordCommandValidator()
    {
        RuleFor(x => x.AppointmentId)
            .NotEmpty();

        RuleFor(x => x.Diagnosis)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(x => x.Notes)
            .MaximumLength(5000);

        RuleFor(x => x.TreatmentPlan)
            .MaximumLength(5000);

        RuleFor(x => x.FollowUpDate)
            .Must(date =>
                !date.HasValue ||
                date.Value.Date >= DateTime.UtcNow.Date)
            .WithMessage(
                "Follow-up date cannot be in the past.");
    }
}
