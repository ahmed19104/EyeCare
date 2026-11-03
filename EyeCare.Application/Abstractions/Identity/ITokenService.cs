namespace EyeCare.Application.Abstractions.Identity;

public interface ITokenService
{
    string CreateAccessToken(
        string userId,
        string email,
        IEnumerable<string> roles);
}