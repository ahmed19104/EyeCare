using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;

using MediatR;

namespace EyeCare.Application.Doctors.Commands.CreateDoctor
{
    public sealed class CreateDoctorCommandHandler
        : IRequestHandler<CreateDoctorCommand, Guid>
    {
        private readonly IRepository<Doctor> _doctorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateDoctorCommandHandler(
            IRepository<Doctor> doctorRepository,
            IUnitOfWork unitOfWork)
        {
            _doctorRepository = doctorRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(
            CreateDoctorCommand request,
            CancellationToken cancellationToken)
        {
            var doctor = new Doctor(
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.Specialty);

            await _doctorRepository.AddAsync(
                doctor,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return doctor.Id;
        }
    }
}
