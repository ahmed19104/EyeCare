using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeCare.Application.Abstractions.Security
{
    public interface IUserService
    {
        Task<UserTokenInfo?> GetTokenInfoAsync(
            string userId,
            CancellationToken cancellationToken = default);
    }

    public sealed record UserTokenInfo(
        string Id,
        string Email);
}
