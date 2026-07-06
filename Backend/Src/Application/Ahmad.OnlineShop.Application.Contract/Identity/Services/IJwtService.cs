using Identity.Domain.Aggregates;

namespace Identity.Application.Services;

public interface IJwtService
{
    (string Token, DateTime ExpiresAt) GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
