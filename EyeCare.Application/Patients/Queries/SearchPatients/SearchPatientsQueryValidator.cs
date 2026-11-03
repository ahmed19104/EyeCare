using FluentValidation;

namespace EyeCare.Application.Patients.Queries.SearchPatients;

public sealed class SearchPatientsQueryValidator
    : AbstractValidator<SearchPatientsQuery>
{
    public SearchPatientsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}
