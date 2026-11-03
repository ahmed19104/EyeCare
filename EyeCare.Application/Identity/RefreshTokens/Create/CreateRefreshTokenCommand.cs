
using EyeCare.Application.Abstractions.Identity;
using MediatR;

namespace EyeCare.Application.Identity.RefreshTokens.Create;

public sealed record CreateRefreshTokenCommand(string UserId)
    : IRequest<RefreshTokenResult>;
