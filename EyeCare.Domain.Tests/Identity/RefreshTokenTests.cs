using EyeCare.Domain.Identity;

namespace EyeCare.Domain.Tests.Identity;

public class RefreshTokenTests
{
    [Fact]
    public void IsActive_ShouldReturnTrue_WhenTokenIsValid()
    {
        // Arrange
        var createdAt = new DateTime(2026, 10, 1, 10, 0, 0, DateTimeKind.Utc);

        var expiresAt = createdAt.AddDays(7);

        var token = new RefreshToken(
            userId: "user-1",
            tokenHash: "hash",
            createdAtUtc: createdAt,
            expiresAtUtc: expiresAt);

        var now = createdAt.AddDays(1);

        // Act
        var result = token.IsActive(now);

        // Assert
        Assert.True(result);
    }
    [Fact]
    public void IsActive_ShouldReturnFalse_WhenTokenIsExpired()
    {
        // Arrange
        var createdAt =
            new DateTime(
                2026, 10, 1, 10, 0, 0,
                DateTimeKind.Utc);

        var token =
            new RefreshToken(
                "user-1",
                "hash",
                createdAt,
                createdAt.AddDays(7));

        var now =
            createdAt.AddDays(7);

        // Act
        var result = token.IsActive(now);

        // Assert
        Assert.False(result);
    }
    [Fact]
    public void IsActive_ShouldReturnFalse_WhenTokenIsRevoked()
    {
        // Arrange
        var createdAt =
            new DateTime(
                2026, 10, 1, 10, 0, 0,
                DateTimeKind.Utc);

        var token =
            new RefreshToken(
                "user-1",
                "hash",
                createdAt,
                createdAt.AddDays(7));

        var now =
            createdAt.AddDays(1);

        token.Revoke(now);

        // Act
        var result = token.IsActive(now);

        // Assert
        Assert.False(result);
    }
    [Fact]
    public void Revoke_ShouldSetRevokedAt()
    {
        // Arrange
        var createdAt =
            new DateTime(
                2026, 10, 1, 10, 0, 0,
                DateTimeKind.Utc);

        var token =
            new RefreshToken(
                "user-1",
                "hash",
                createdAt,
                createdAt.AddDays(7));

        var revokedAt =
            createdAt.AddDays(1);

        // Act
        token.Revoke(revokedAt);

        // Assert
        Assert.Equal(
            revokedAt,
            token.RevokedAtUtc);
    }
    [Fact]
    public void Revoke_ShouldStoreReplacementTokenHash()
    {
        // Arrange
        var createdAt =
            new DateTime(
                2026, 10, 1, 10, 0, 0,
                DateTimeKind.Utc);

        var token =
            new RefreshToken(
                "user-1",
                "old-hash",
                createdAt,
                createdAt.AddDays(7));

        var replacementHash = "new-hash";

        var revokedAt =
            createdAt.AddDays(1);

        // Act
        token.Revoke(
            revokedAt,
            replacementHash);

        // Assert
        Assert.Equal(
            replacementHash,
            token.ReplacedByTokenHash);
    }
    [Fact]
    public void Revoke_ShouldNotChangeRevokedAt_WhenTokenIsAlreadyRevoked()
    {
        // Arrange
        var createdAt =
            new DateTime(
                2026, 10, 1, 10, 0, 0,
                DateTimeKind.Utc);

        var token =
            new RefreshToken(
                "user-1",
                "hash",
                createdAt,
                createdAt.AddDays(7));

        var firstRevokedAt =
            createdAt.AddDays(1);

        var secondRevokedAt =
            createdAt.AddDays(2);

        token.Revoke(firstRevokedAt);

        // Act
        token.Revoke(secondRevokedAt);

        // Assert
        Assert.Equal(
            firstRevokedAt,
            token.RevokedAtUtc);
    }
}