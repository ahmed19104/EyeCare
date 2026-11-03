using FluentValidation;

namespace EyeCare.Application.MedicalRecords.Queries.GetPatientMedicalHistory;

public sealed class GetPatientMedicalHistoryQueryValidator
    : AbstractValidator<GetPatientMedicalHistoryQuery>
{
    public GetPatientMedicalHistoryQueryValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty();

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}
