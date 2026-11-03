using EyeCare.Application.Abstractions.Identity;
using MediatR;

namespace EyeCare.Application.Identity.Commands.LoginUser;

public sealed record LoginUserCommand(
    string Email,
    string Password)
    : IRequest<LoginResult>;