using EyeCare.Application.Appointments.Comends.BookAppointment;
using FluentValidation;

namespace EyeCare.Application.Appointments.Comends.BookAppointment;

public sealed class BookAppointmentCommandValidator
    : AbstractValidator<BookAppointmentCommand>
{
    public BookAppointmentCommandValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty()
            .WithMessage("Patient is required.");

        RuleFor(x => x.DoctorId)
            .NotEmpty()
            .WithMessage("Doctor is required.");

        RuleFor(x => x.BranchId)
            .NotEmpty()
            .WithMessage("Branch is required.");

        RuleFor(x => x.Date)
           .NotEmpty();

        RuleFor(x => x.StartTime)
            .GreaterThanOrEqualTo(TimeSpan.Zero)
            .LessThan(TimeSpan.FromDays(1));


    }
}
