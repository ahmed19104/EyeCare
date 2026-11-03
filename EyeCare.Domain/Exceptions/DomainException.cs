using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeCare.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message)
            : base(message)
        {
        }
    
       public DomainException(string message, string paramName)
                : base($"{message} (Parameter '{paramName}')")
            {
            }
        }
}
