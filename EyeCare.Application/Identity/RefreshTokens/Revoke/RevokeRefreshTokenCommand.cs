using MediatR;

namespace EyeCare.Application.Identity.RefreshTokens.Revoke;

public sealed record RevokeRefreshTokenCommand(
    string RefreshToken
) : IRequest<Unit>;