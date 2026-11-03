using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;
using System.Linq.Expressions;


namespace EyeCare.Application.Appointments.Comends.BookAppointment.Specifications
{
    public sealed class PatientByIdSpecification
        : ISpecification<Patient>
    {
        public PatientByIdSpecification(Guid patientId)
        {
            Criteria = patient => patient.Id == patientId;
        }

        public Expression<Func<Patient, bool>> Criteria { get; }
        public List<Expression<Func<Patient, object>>> Includes { get; } = new List<Expression<Func<Patient, object>>>();
        public Expression<Func<Patient, object>>?
    OrderByDescending
        { get; } = null;
    }
}
