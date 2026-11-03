using MediatR;

namespace EyeCare.Application.Identity.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password)
    : IRequest<string>;