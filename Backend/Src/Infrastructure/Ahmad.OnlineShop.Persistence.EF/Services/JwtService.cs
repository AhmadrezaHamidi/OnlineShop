using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Identity.Application.Services;
using Identity.Domain.Aggregates;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Ahmad.OnlineShop.Persistence.EF.Services;

public sealed class JwtService(IConfiguration configuration) : IJwtService
{
    private readonly string  _secret    = configuration["Jwt:Secret"]     ?? throw new InvalidOperationException("Jwt:Secret not configured");
    private readonly string  _issuer    = configuration["Jwt:ValidIssuer"] ?? "AhmadOnlineShop";
    private readonly string  _audience  = configuration["Jwt:ValidAudience"] ?? "AhmadOnlineShop";
    private readonly int     _expMinutes = int.TryParse(configuration["Jwt:TokenExpireMinutes"], out var m) ? m : 60;

    public (string Token, DateTime ExpiresAt) GenerateAccessToken(User user)
    {
        var key     = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var creds   = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_expMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,  user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti,  Guid.NewGuid().ToString()),
            new("phone", user.PhoneNumber),
        };

        if (!string.IsNullOrEmpty(user.FullName))
            claims.Add(new Claim(JwtRegisteredClaimNames.Name, user.FullName));

        foreach (var roleId in user.RoleIds)
            claims.Add(new Claim("role_id", roleId.ToString()));

        var token = new JwtSecurityToken(
            issuer:             _issuer,
            audience:           _audience,
            claims:             claims,
            notBefore:          DateTime.UtcNow,
            expires:            expires,
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }

    public string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}
