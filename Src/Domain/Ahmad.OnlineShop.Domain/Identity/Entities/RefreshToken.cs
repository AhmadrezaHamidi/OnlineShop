using Identity.Domain.Exceptions;

namespace Identity.Domain.Entities;

public sealed class RefreshToken
{
    public long Id { get; private set; }
    public long UserId { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }

    private RefreshToken() { }

    public RefreshToken(long id, long userId, string token, DateTime expiresAt)
    {
        Id = id;
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
    }

    public void EnsureValid()
    {
        if (IsRevoked || DateTime.UtcNow > ExpiresAt)
            throw new InvalidRefreshTokenException();
    }

    public void Revoke() => IsRevoked = true;
}
