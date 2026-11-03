using FluentValidation;

namespace EyeCare.Application.Appointments.Queries.GetPatientAppointments;

public sealed class GetPatientAppointmentsQueryValidator
    : AbstractValidator<GetPatientAppointmentsQuery>
{
    public GetPatientAppointmentsQueryValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty();

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}