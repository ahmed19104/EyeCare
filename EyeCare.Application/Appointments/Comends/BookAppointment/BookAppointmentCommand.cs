using MediatR;


namespace EyeCare.Application.Appointments.Comends.BookAppointment
{
    public sealed record BookAppointmentCommand(
     Guid PatientId,
     Guid DoctorId,
     Guid BranchId,
     DateOnly Date,
     TimeSpan StartTime) : IRequest<Guid>;
}
