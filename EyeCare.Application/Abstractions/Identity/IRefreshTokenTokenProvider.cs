
namespace EyeCare.Application.Abstractions.Identity;

public interface IRefreshTokenTokenProvider
{
    string GenerateToken();

    string HashToken(string token);
}
