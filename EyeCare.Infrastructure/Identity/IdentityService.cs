
using EyeCare.Application.Abstractions.Identity;
using EyeCare.Application.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace EyeCare.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        IRefreshTokenService refreshTokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<string> CreateUserAsync(
        string firstName,
        string lastName,
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var existingUser =
            await _userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            throw new ConflictException(
                "A user with this email already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };

        var result = await _userManager.CreateAsync(
            user,
            password);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                "; ",
                result.Errors.Select(x => x.Description));

            throw new ConflictException(errors);
        }

        return user.Id;
    }

    public async Task<LoginResult?> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user =
            await _userManager.FindByEmailAsync(email);

        if (user is null)
            return null;

        var signInResult =
            await _signInManager.CheckPasswordSignInAsync(
                user,
                password,
                lockoutOnFailure: true);

        if (!signInResult.Succeeded)
            return null;

        var roles =
            await _userManager.GetRolesAsync(user);

        var accessToken =
            _tokenService.CreateAccessToken(
                user.Id,
                user.Email!,
                roles);

        var refreshToken =
    await _refreshTokenService.CreateAsync(
        user.Id,
        cancellationToken);

       
        return new LoginResult(
            user.Id,
            refreshToken,
            accessToken);
    }
}