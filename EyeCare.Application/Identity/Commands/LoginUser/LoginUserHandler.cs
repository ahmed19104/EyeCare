using EyeCare.Application.Abstractions.Identity;
using EyeCare.Application.Exceptions;
using MediatR;

namespace EyeCare.Application.Identity.Commands.LoginUser;

public sealed class LoginUserHandler
    : IRequestHandler<LoginUserCommand, LoginResult>
{
    private readonly IIdentityService _identityService;

    public LoginUserHandler(
        IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<LoginResult> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        var result =
            await _identityService.LoginAsync(
                request.Email,
                request.Password,
                cancellationToken);

        if (result is null)
        {
            throw new UnauthorizedException(
                "Invalid email or password.");
        }

        return result;
    }
}