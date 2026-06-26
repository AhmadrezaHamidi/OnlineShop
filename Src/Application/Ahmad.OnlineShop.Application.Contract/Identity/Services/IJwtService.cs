using Ahmad.OnlineShop.Domain.User;

namespace Identity.Application.Services;

public interface IJwtService
{
    /// <summary>Generates a signed JWT access token and returns it with its expiry time.</summary>
    (string Token, DateTime ExpiresAt) GenerateAccessToken(User user);

    /// <summary>Generates a cryptographically random refresh token string.</summary>
    string GenerateRefreshToken();
}
