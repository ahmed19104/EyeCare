
using EyeCare.Application.Abstractions.Identity;
using System.Security.Cryptography;
using System.Text;

namespace EyeCare.Infrastructure.Identity;

public  class RefreshTokenCryptography: IRefreshTokenTokenProvider
{
    public  string GenerateToken()
    {
        
        return Convert.ToHexString(
            RandomNumberGenerator.GetBytes(64));
    }

    public  string HashToken(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = SHA256.HashData(tokenBytes);

        return Convert.ToHexString(hashBytes);
    }
}
