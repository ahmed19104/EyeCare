namespace EyeCare.Application.Identity.RefreshTokens;

public sealed record RefreshTokenResult(
    string AccessToken,
    string RefreshToken);
