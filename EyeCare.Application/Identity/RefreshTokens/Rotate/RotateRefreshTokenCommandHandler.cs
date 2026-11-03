using EyeCare.Application.Abstractions.Identity;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Abstractions.Security;
using EyeCare.Application.Exceptions;
using EyeCare.Application.Identity.RefreshTokens;
using EyeCare.Application.Identity.RefreshTokens.Specifications;
using EyeCare.Domain.Identity;
using MediatR;


namespace EyeCare.Application.Identity.RefreshTokens.Rotate;

public sealed class RotateRefreshTokenCommandHandler
    : IRequestHandler<RotateRefreshTokenCommand, RefreshTokenResult>
{
    private readonly IRepository<RefreshToken> _refreshTokenRepository;
    private readonly IRefreshTokenTokenProvider _tokenProvider;
    private readonly ITokenService _tokenService;
    private readonly IRoleService _roleService;
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRefreshTokenSettings _settings;
    private readonly TimeProvider _timeProvider;

    public RotateRefreshTokenCommandHandler(
        IRepository<RefreshToken> refreshTokenRepository,
        IRefreshTokenTokenProvider tokenProvider,
        ITokenService tokenService,
        IRoleService roleService,
        IUserService userService,
        IUnitOfWork unitOfWork,
        IRefreshTokenSettings settings,
        TimeProvider timeProvider)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _tokenProvider = tokenProvider;
        _tokenService = tokenService;
        _roleService = roleService;
        _userService = userService;
        _unitOfWork = unitOfWork;
        _settings = settings;
        _timeProvider = timeProvider;
    }

    public async Task<RefreshTokenResult> Handle(
        RotateRefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            throw new UnauthorizedException(
                "Refresh token is required.");
        }

        var now = _timeProvider.GetUtcNow().UtcDateTime;

        // 1. Hash الـ token اللي جاي من العميل
        var tokenHash = _tokenProvider.HashToken(
            request.RefreshToken);

        // 2. نجيب الـ RefreshToken من DB
        var currentToken =
            await _refreshTokenRepository.FirstOrDefaultAsync(
                new RefreshTokenHashSpecification(tokenHash),
                cancellationToken);

        // 3. التوكن مش موجود
        if (currentToken is null)
        {
            throw new UnauthorizedException(
                "Invalid refresh token.");
        }

        // 4. التوكن expired أو revoked
        if (!currentToken.IsActive(now))
        {
            throw new UnauthorizedException(
                "Refresh token is expired or revoked.");
        }

        // 5. نجيب بيانات المستخدم
        var user =
            await _userService.GetTokenInfoAsync(
                currentToken.UserId,
                cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedException(
                "User no longer exists.");
        }

        // 6. نجيب الـ Roles
        var roles =
            await _roleService.GetUserRolesAsync(
                currentToken.UserId,
                cancellationToken);

        // 7. نعمل Refresh Token جديد
        var newRawRefreshToken =
            _tokenProvider.GenerateToken();

        var newRefreshTokenHash =
            _tokenProvider.HashToken(
                newRawRefreshToken);

        var replacement = new RefreshToken(
            currentToken.UserId,
            newRefreshTokenHash,
            now,
            now.AddDays(_settings.LifetimeDays));

        // 8. Revoke القديم
        currentToken.Revoke(
            now,
            newRefreshTokenHash);

        // 9. Add الجديد
        await _refreshTokenRepository.AddAsync(
            replacement,
            cancellationToken);

        // 10. نحفظ الاتنين مع بعض
        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        // 11. نصدر Access Token جديد
        var accessToken =
            _tokenService.CreateAccessToken(
                user.Id,
                user.Email,
                roles);

        return new RefreshTokenResult(
            accessToken,
            newRawRefreshToken);
    }
}