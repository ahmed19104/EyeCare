
using EyeCare.Domain.Exceptions;

namespace EyeCare.Domain.Identity;

public sealed class RefreshToken
{
    public byte[] RowVersion { get; private set; } = null!;

    public Guid Id { get; private set; }

    public string UserId { get; private set; } = null!;

    // Store the hash, never the original refresh token.
    public string TokenHash { get; private set; } = null!;

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime ExpiresAtUtc { get; private set; }

    public DateTime? RevokedAtUtc { get; private set; }

    // Hash of the replacement token, if this token was rotated.
    public string? ReplacedByTokenHash { get; private set; }

    private RefreshToken()
    {
        // Required by EF Core.
    }

    public RefreshToken(
        string userId,
        string tokenHash,
        DateTime createdAtUtc,
        DateTime expiresAtUtc)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new DomainException("User ID is required.", nameof(userId));

        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new DomainException("Token hash is required.", nameof(tokenHash));

        if (expiresAtUtc <= createdAtUtc)
            throw new DomainException(
                "Expiration must be later than creation.",
                nameof(expiresAtUtc));

        Id = Guid.NewGuid();
        UserId = userId;
        TokenHash = tokenHash;
        CreatedAtUtc = createdAtUtc;
        ExpiresAtUtc = expiresAtUtc;
    }

    public bool IsExpired(DateTime utcNow)
        => utcNow >= ExpiresAtUtc;

    public bool IsRevoked
        => RevokedAtUtc.HasValue;

    public bool IsActive(DateTime utcNow)
        => !IsRevoked && !IsExpired(utcNow);

    public void Revoke(DateTime utcNow, string? replacedByTokenHash = null)
    {
        if (IsRevoked)
            return;

        if (string.IsNullOrWhiteSpace(replacedByTokenHash) is false)
            ReplacedByTokenHash = replacedByTokenHash;

        RevokedAtUtc = utcNow;
    }
}