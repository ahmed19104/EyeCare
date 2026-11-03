using MediatR;
namespace EyeCare.Application.Doctors.Commands.CreateDoctor
{
    public sealed record CreateDoctorCommand(
        string FirstName,
        string LastName,
        string PhoneNumber,
        string Specialty) : IRequest<Guid>;
}
