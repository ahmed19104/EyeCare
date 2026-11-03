using FluentValidation;

namespace EyeCare.Application.Appointments.Queries.GetDoctorAppointments;

public sealed class GetDoctorAppointmentsQueryValidator
    : AbstractValidator<GetDoctorAppointmentsQuery>
{
    public GetDoctorAppointmentsQueryValidator()
    {
        RuleFor(x => x.DoctorId)
            .NotEmpty();

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}