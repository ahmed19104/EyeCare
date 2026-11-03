using EyeCare.Application.Abstractions.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeCare.Infrastructure.Identity
{
    public class RefreshTokenSettingsAdapter: IRefreshTokenSettings
    {
        private readonly RefreshTokenOptions _options;

        public RefreshTokenSettingsAdapter(IOptions<RefreshTokenOptions> options)
        {
            _options = options.Value;
        }

        public int LifetimeDays => _options.LifetimeDays;
    }
}
