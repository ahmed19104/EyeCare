using EyeCare.Application.Abstractions.Identity;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Identity;
using Microsoft.Extensions.Options;

namespace EyeCare.Infrastructure.Identity;

public sealed class RefreshTokenService
    : IRefreshTokenService
{
    private readonly IRepository<RefreshToken> _repository;
    private readonly IRefreshTokenTokenProvider _tokenProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRefreshTokenSettings _settings;
    private readonly TimeProvider _timeProvider;

    public RefreshTokenService(
        IRepository<RefreshToken> repository,
        IRefreshTokenTokenProvider tokenProvider,
        IUnitOfWork unitOfWork,
        IRefreshTokenSettings settings,
        TimeProvider timeProvider)
    {
        _repository = repository;
        _tokenProvider = tokenProvider;
        _unitOfWork = unitOfWork;
        _settings = settings;
        _timeProvider = timeProvider;
    }

    public async Task<string> CreateAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var now =
            _timeProvider
                .GetUtcNow()
                .UtcDateTime;

        var rawToken =
            _tokenProvider.GenerateToken();

        var tokenHash =
            _tokenProvider.HashToken(rawToken);

        var refreshToken =
            new RefreshToken(
                userId,
                tokenHash,
                now,
                now.AddDays(
                    _settings.LifetimeDays));

        await _repository.AddAsync(
            refreshToken,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return rawToken;
    }
}
