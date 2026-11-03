
using EyeCare.Application.Abstractions.Identity;
using MediatR;



namespace EyeCare.Application.Identity.RefreshTokens.Rotate;

public sealed record RotateRefreshTokenCommand(
    string RefreshToken
) : IRequest<RefreshTokenResult>;