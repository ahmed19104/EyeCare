namespace EyeCare.Application.Abstractions.Identity;

public sealed record LoginResult(
    string UserId,
    string RefreshToken,
    string AccessToken);