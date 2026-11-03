using FluentValidation;

namespace EyeCare.Application.MedicalRecords.Commands.UpdateMedicalRecord;

public sealed class UpdateMedicalRecordCommandValidator
    : AbstractValidator<UpdateMedicalRecordCommand>
{
    public UpdateMedicalRecordCommandValidator()
    {
        RuleFor(x => x.MedicalRecordId)
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