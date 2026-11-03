
using EyeCare.Application.Abstractions.Identity;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Identity;
using MediatR;


namespace EyeCare.Application.Identity.RefreshTokens.Create;

public sealed class CreateRefreshTokenCommandHandler
    : IRequestHandler<CreateRefreshTokenCommand, RefreshTokenResult>
{
    private readonly IRepository<RefreshToken>  _repository;
    private readonly IRefreshTokenTokenProvider _tokenProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRefreshTokenSettings _refreshTokenSettings;
    private readonly TimeProvider _timeProvider;

    public CreateRefreshTokenCommandHandler(
        IRepository<RefreshToken> repository,
        IRefreshTokenTokenProvider tokenProvider,
        IUnitOfWork unitOfWork,
        IRefreshTokenSettings refreshTokenSettings,
        TimeProvider timeProvider)
    {
        _repository = repository;
        _tokenProvider = tokenProvider;
        _unitOfWork = unitOfWork;
        _refreshTokenSettings = refreshTokenSettings;
        
        _timeProvider = timeProvider;
    }

    public async Task<RefreshTokenResult> Handle(
        CreateRefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);

      

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var rawToken = _tokenProvider.GenerateToken();
        var tokenHash = _tokenProvider.HashToken(rawToken);

        var entity = new RefreshToken(
            request.UserId,
            tokenHash,
            now,
            now.AddDays(_refreshTokenSettings.LifetimeDays));

        await _repository.AddAsync(entity, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResult(request.UserId, rawToken);
    }
}
