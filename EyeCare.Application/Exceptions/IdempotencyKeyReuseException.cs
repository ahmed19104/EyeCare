using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeCare.Application.Exceptions
{
    public sealed class IdempotencyKeyReuseException
    : Exception
    {
        public IdempotencyKeyReuseException()
            : base(
                "The idempotency key was already used for a different payment request.")
        {
        }
    }
}
