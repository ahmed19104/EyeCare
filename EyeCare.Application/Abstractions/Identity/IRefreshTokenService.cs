namespace EyeCare.Application.Abstractions.Identity;

public interface IRefreshTokenService
{
    Task<string> CreateAsync(
        string userId,
        CancellationToken cancellationToken = default);
}
