namespace EyeCare.Application.Abstractions.Identity;

public interface IIdentityService
{
    Task<string> CreateUserAsync(
        string firstName,
        string lastName,
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task<LoginResult?> LoginAsync(
    string email,
    string password,
    CancellationToken cancellationToken = default);
}
