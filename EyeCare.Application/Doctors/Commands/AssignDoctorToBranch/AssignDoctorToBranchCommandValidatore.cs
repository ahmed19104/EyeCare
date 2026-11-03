using EyeCare.Application.Doctors.Commands.CreateDoctor;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeCare.Application.Doctors.Commands.AssignDoctorToBranch
{
    public sealed class CreateDoctorCommandValidator
        : AbstractValidator<AssignDoctorToBranchCommand>
    {
        public CreateDoctorCommandValidator()
        {
            RuleFor(x => x.BranchId)
                .NotEmpty();


            RuleFor(x => x.DoctorId)
                .NotEmpty();
                

            
        }
    }
}
