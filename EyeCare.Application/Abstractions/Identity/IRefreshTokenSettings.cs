using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeCare.Application.Abstractions.Identity
{
    public interface IRefreshTokenSettings
    {
        int LifetimeDays { get; }
    }
}
