using EyeCare.Application.Abstractions.Identity;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Identity.RefreshTokens.Specifications;
using EyeCare.Domain.Identity;
using MediatR;

namespace EyeCare.Application.Identity.RefreshTokens.Revoke;

public sealed class RevokeRefreshTokenCommandHandler
    : IRequestHandler<
        RevokeRefreshTokenCommand,
        Unit>
{
    private readonly IRepository<RefreshToken> 
        _refreshTokenRepository;

    private readonly IRefreshTokenTokenProvider
        _tokenProvider;

    private readonly IUnitOfWork
        _unitOfWork;

    private readonly TimeProvider
        _timeProvider;

    public RevokeRefreshTokenCommandHandler(
        IRepository<RefreshToken> refreshTokenRepository,
        IRefreshTokenTokenProvider tokenProvider,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
    {
        _refreshTokenRepository =
            refreshTokenRepository;

        _tokenProvider =
            tokenProvider;

        _unitOfWork =
            unitOfWork;

        _timeProvider =
            timeProvider;
    }

    public async Task<Unit> Handle(
        RevokeRefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(
            request.RefreshToken))
        {
            return Unit.Value;
        }

        var hash =
            _tokenProvider.HashToken(
                request.RefreshToken);

        var token =
            await _refreshTokenRepository
                .FirstOrDefaultAsync(
                    new RefreshTokenHashSpecification(hash),
                    cancellationToken);

        if (token is null)
            return Unit.Value;

        var now =
            _timeProvider
                .GetUtcNow()
                .UtcDateTime;

        if (!token.IsActive(now))
            return Unit.Value;

        token.Revoke(now);

        await _unitOfWork
            .SaveChangesAsync(
                cancellationToken);

        return Unit.Value;
    }
}
