using EyeCare.Application.Abstractions.Identity;
using EyeCare.Application.Exceptions;
using MediatR;


namespace EyeCare.Application.Identity.Commands.RegisterUser;

public sealed class RegisterUserHandler
    : IRequestHandler<RegisterUserCommand, string>
{
    private readonly IIdentityService _identityService;

    public RegisterUserHandler(
        IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<string> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var userId =
    await _identityService.CreateUserAsync(
        request.FirstName,
        request.LastName,
        request.Email,
        request.Password,
        cancellationToken);
        return userId;
    }
}
